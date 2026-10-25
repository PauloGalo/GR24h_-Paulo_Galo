using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using GR24h_Repos;

namespace GR24h_Services
{
    public class UserFavouritesService
    {
        private readonly UserFavouritesRepository _userFavRepo;
        private readonly UsersRepository _userRepo;
        private readonly RecipeService _recipeService;
        

        public UserFavouritesService(
            UserFavouritesRepository userFavRepo,
            UsersRepository userRepo, 
            RecipeService recipeService
            )
        {
            _userFavRepo = userFavRepo;
            _userRepo = userRepo;
            _recipeService = recipeService;
           
        }

       
        public bool ToggleFavourite(int userId, int recipeId)
        {
            

            Users? user = _userRepo.Retrieve(userId);

           
            if (user == null || user.IsLocked || user.IsAdmin)
            {
                throw new Exception("Operação Falhada : Utilizador não existe, utilizador bloqueado ou utilizador é administrador.");
            }

            Recipe? recipe = _recipeService.GetRecipeById(recipeId);
            if (recipe == null)
            {
                throw new Exception("Operação Falhada : Receita não existe.");
            }

            UserFavourites? existingFav = _userFavRepo.RetrieveAll()
                .FirstOrDefault(f => f.RecipeId == recipeId && f.UserId == userId);

            if (existingFav != null)
            {
                _userFavRepo.Delete(existingFav.Id);
                return false; 
            }
            else
            {
                UserFavourites newFavourite = UserFavourites.CreateFavourite(userId, recipeId);
                _userFavRepo.Create(newFavourite);
                return true;
            }
        }



        public UserFavourites GetUserFavById(int favId)
        {
            if (favId <= 0)
                throw new Exception("Operação Falhada : ID de Favorito inválido.");

            UserFavourites? favourite = _userFavRepo.Retrieve(favId);

            if (favourite == null)
                throw new Exception("Operação Falhada : Favorito solicitado não existe.");

            return favourite;

        }

        public List<Recipe> GetFavouriteRecipes(int userId)
        {

            Users? user = _userRepo.Retrieve(userId);
            if (user == null || user.IsAdmin || user.IsLocked)
            {
                return new List<Recipe>();
            }

            List<int> favouriteRecipeIds = _userFavRepo.RetrieveAll()
                .Where(f => f.UserId == userId)
                .Select(f => f.RecipeId)
                .ToList();

            List<Recipe> favouriteRecipes = new List<Recipe>();

            foreach (int id in favouriteRecipeIds)
            {
                Recipe? r = _recipeService.GetRecipeById(id);
                if (r != null)
                {
                    favouriteRecipes.Add(r);
                }
            }

            return favouriteRecipes;
        }


        public bool IsFavourite(int recipeId, int userId)
        {

            Users? user = _userRepo.Retrieve(userId);

            if (user == null || user.IsAdmin || user.IsLocked)
            {
                return false;
            }

            return _userFavRepo.RetrieveAll()
                .Any(f => f.RecipeId == recipeId && f.UserId == userId);
        }


        public void RemoveAllFavsByUserId(int userId)
        {
            List<UserFavourites> userFavs = _userFavRepo.RetrieveAll()
                .Where(f => f.UserId == userId)
                .ToList();

            foreach (UserFavourites fav in userFavs)
            {
                _userFavRepo.Delete(fav.Id);
            }
        }

        public int GetFavoriteCount(int recipeId)
        {
            return _userFavRepo.RetrieveAll()
                .Count(f => f.RecipeId == recipeId);
        }

        
        public Dictionary<int, int> GetAllFavoriteCounts()
        {
            return _userFavRepo.RetrieveAll()
                .GroupBy(f => f.RecipeId)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
