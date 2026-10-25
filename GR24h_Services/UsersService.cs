using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using GR24h_Repos;

namespace GR24h_Services
{
    public record class RegisterRequest(
      string FirstName,
      string LastName,
      string Username,
      string Email,
      string Password,
      bool IsAdmin);

    public record class LoginRequest(
         string Username,
         string Password);


    public class UsersService
    {
        private readonly UsersRepository _userRepo;
        private readonly RecipeService _recipeService; 
        private readonly RatingService _ratingService;
        private readonly CommentService _commentService;
        private readonly UserFavouritesService _favService;

        public UsersService(
            UsersRepository userRepo,
            RecipeService recipeService,
            RatingService ratingService,
            CommentService commentService,
            UserFavouritesService favService
            )
        {
            _userRepo = userRepo;
            _recipeService = recipeService;
            _ratingService = ratingService;
            _commentService = commentService;
            _favService = favService;
        }


        public Users Register(RegisterRequest request)
        {
            Users newUser = new Users(
                0,
                request.FirstName,
                request.LastName,
                request.Username,
                request.Email,
                request.Password,
                request.IsAdmin,
                false,
                0
            );




            List<Users> users = _userRepo.RetrieveAll();

            if (users.Any(u => u.Email.Equals(newUser.Email, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("Operação Falhada : Email inserido pertence a outro utilizador. Tente novamente.");

            if (users.Any(u => u.Username.Equals(newUser.Username, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("Operação Falhada : Username inserido pertence a outro utilizador. Tente novamente.");


            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
            newUser.SetPasswordHash(hashedPassword);


            _userRepo.Create(newUser);


            return newUser;
        }


        public Users? Login(LoginRequest reqLogin)
        {

            List<Users> allUsers = _userRepo.RetrieveAll();
            Users? foundUser = allUsers.FirstOrDefault(u => u.Username.Equals(reqLogin.Username, StringComparison.OrdinalIgnoreCase));

            if (foundUser == null)
            {
                return null;

            }


            if (foundUser.IsLocked)

            {
                throw new Exception("Operação Falhada : Esta conta está bloqueada. Contacte à Administração.");

            }

            if (foundUser.FailedLoginAttempts >= 5)
            {
                throw new Exception("Operação Falhada : Acesso bloqueado. Máximo de tentativas alcançado. Contacte à Administração.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(reqLogin.Password, foundUser.Password);

            if (isPasswordValid)
            {
                foundUser.ResetFailedAttempts();
                _userRepo.Update(foundUser);
                return foundUser;
            }
            else
            {
                foundUser.IncrementFailedAttempts();
                _userRepo.Update(foundUser);

                if (foundUser.IsLocked)
                {
                    throw new Exception("Operação Falhada : Esta conta está bloqueada. Contacte à Administração.");

                }


                return null;
            }
        }


        public Users? GetUserById(int userId)
        {
            if (userId <= 0)
                throw new Exception("Operação Falhada : ID de utilizador inválido.");

            Users? user = _userRepo.Retrieve(userId);

            if (user == null)
                throw new Exception("Operação Falhada : Utilizador solicitado não existe.");

            return user;
        }


        public List<Users> GetAllUsers()
        {
            return _userRepo.RetrieveAll();
        }

        public void UpdateProfile(int id, string firstName, string lastName, string username, string email, string? newPassword)
        {

            Users? current = _userRepo.Retrieve(id);
            if (current == null) throw new Exception("Operação Falhada : Utilizador não existe.");


            List<Users> allUsers = _userRepo.RetrieveAll();
            if (allUsers.Any(u => u.Id != id && u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("Operação Falhada : Email inserido pertence a outro utilizador. Tente novamente.");

            if (allUsers.Any(u => u.Id != id && u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("Operação Falhada : Username inserido pertence a outro utilizador. Tente novamente.");


            string passwordToValidate = string.IsNullOrWhiteSpace(newPassword) ? current.Password : newPassword;


            Users updatedUser = new Users(
                id,
                firstName,
                lastName,
                username,
                email,
                passwordToValidate,
                current.IsAdmin,
                current.IsLocked,
                current.FailedLoginAttempts
            );

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                updatedUser.SetPasswordHash(hashedPassword);
            }


            _userRepo.Update(updatedUser);
        }
        public void ToggleUserLock(int administratorId, int targetUserId)
        {
            Users? administrator = _userRepo.Retrieve(administratorId);

            if (administrator == null || !administrator.IsAdmin)
            {
                throw new Exception("Operação Negada : Somente Administradores tem permissão para bloquear e desbloquear contas.");
            }


            Users? targetUser = _userRepo.Retrieve(targetUserId);

            if (targetUser == null)
            {
                throw new Exception("Operação Falhada : Utilizador não existe.");
            }


            if (administrator.Id == targetUser.Id)
            {
                throw new Exception("Operação Falhada : Contas de Admnistradores não podem ser bloqueadas.");
            }


            targetUser.UpdateLockStatus(!targetUser.IsLocked);


            _userRepo.Update(targetUser);
        }


        private void DeleteUserDependencies(int userId)
        {
           
            _favService.RemoveAllFavsByUserId(userId);

            
            _ratingService.RemoveAllRatingsByUserId(userId);

            
            _commentService.RemoveAllCommentsByUserId(userId);

            _recipeService.DeleteRecipesByUserId(userId);
        }

        public void UserDeleteAccount(int userId, string passwordConfirm)
        {
            Users? user = _userRepo.Retrieve(userId);
            if (user == null) throw new Exception("Operação Falhada : Utilizador não existe.");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(passwordConfirm, user.Password);

            if (!isPasswordValid)
            {
                throw new Exception("Operação Falhada : Acesso negado. Palavra-Passe inserida inválida.");
            }

            if (user.IsAdmin)
            {
                List<Users> allUsers = _userRepo.RetrieveAll();
                int adminCount = allUsers.Count(u => u.IsAdmin);

                if (adminCount <= 1)
                {
                    throw new Exception("Operação Falhada : Última conta de admistrador. " +
                                        "É obrigatório existir no mínimo um administrador em sistema.");
                }
            }

            DeleteUserDependencies(userId);

            _userRepo.Delete(userId);
        }

        public void AdminDeleteUser(int administratorId, int targetUserId)
        {
            Users? administrator = _userRepo.Retrieve(administratorId);
            if (administrator == null || !administrator.IsAdmin)
                throw new Exception("Operação Falhada : Somente Admistradores tem permissão para apagar utilizadores em sistema.");


            Users? targetUser = _userRepo.Retrieve(targetUserId);
            if (targetUser == null)
                throw new Exception("Operação Falhada : Utilizador não existe.");


            if (targetUser.IsAdmin)
            {
                List<Users> allUsers = _userRepo.RetrieveAll();
                int adminAccount = allUsers.Count(u => u.IsAdmin);


                if (adminAccount <= 1)
                {
                    throw new Exception("Operação Falhada : Última conta de admistrador. " +
                                        "É obrigatório existir no mínimo um administrador em sistema.");
                }
            }

            
            DeleteUserDependencies(targetUserId);

            _userRepo.Delete(targetUserId);
        }

    }
}
