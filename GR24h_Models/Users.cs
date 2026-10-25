using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR24h_Models
{
    public class Users
    {

        public int Id { get; private set; }

        public string FirstName { get; private set; } = null!;

        public string LastName { get; private set; } = null!;

        public string Email { get; private set; } = null!;

        public string Username { get; private set; } = null!;

        public string Password { get; private set; } = null!;

        public bool IsAdmin { get; private set; }

        public bool IsLocked { get; private set; }

        public int FailedLoginAttempts { get; private set; } = 0;

        public Users(
        int id,
        string firstName,
        string lastName,
        string username,
        string email,
        string password,
        bool isAdmin,
        bool isLocked,
        int failedLoginAttempts = 0)
        {

            this.Id = id;
            this.FirstName = ValidateFirstName(firstName);
            this.LastName = ValidateLastName(lastName);
            this.Username = ValidateUsername(username);
            this.Email = ValidateUserEmail(email);
            this.IsAdmin = ValidateUserIsAdmin(this.Username, this.Email);
            this.Password = ValidatePassword(password);
            this.IsLocked = isLocked;
            this.FailedLoginAttempts = failedLoginAttempts;
        }



        private string ValidateFirstName(string valueFirstName)
        {
            if (string.IsNullOrWhiteSpace(valueFirstName))
                throw new ArgumentException("Campo obrigatório");

            valueFirstName = valueFirstName.Trim();

            if (valueFirstName.Length < 2 || valueFirstName.Length > 15)
                throw new ArgumentException("Nome deve conter entre 2 a 15 caracteres.Tente novamente.");

            if (!valueFirstName.All(char.IsLetter))
                throw new ArgumentException("Nome deve conter apenas letras.Tente novamente.");

            return valueFirstName;
        }


        private string ValidateLastName(string valueLastName)
        {
            if (string.IsNullOrWhiteSpace(valueLastName))
                throw new ArgumentException("Campo obrigatório");

            valueLastName = valueLastName.Trim();

            if (valueLastName.Length < 2 || valueLastName.Length > 15)
                throw new ArgumentException("Apelido deve conter entre 2 a 15 caracteres.Tente novamente.");

            if (!valueLastName.All(char.IsLetter))
                throw new ArgumentException("Apelido deve conter apenas letras.Tente novamente.");

            return valueLastName;
        }


        private string ValidateUsername(string valueUsername)
        {


            if (string.IsNullOrWhiteSpace(valueUsername))
                throw new ArgumentException("Campo obrigatório");

            valueUsername = valueUsername.Trim();

            if (valueUsername.Length < 3 || valueUsername.Length > 15)
                throw new ArgumentException("Username deve conter entre 3 a 15 caracteres.Tente novamente.");



            foreach (char c1 in valueUsername)
            {
                if (!char.IsLetterOrDigit(c1) && c1 != '_')
                    throw new ArgumentException("Username deve conter apenas letras, números e underscore.Tente novamente.");
            }

            return valueUsername;
        }





        private string ValidateUserEmail(string valueEmail)
        {


            if (string.IsNullOrWhiteSpace(valueEmail))
                throw new ArgumentException("Campo obrigatório");

            valueEmail = valueEmail.Trim().ToLower();

            if (valueEmail.Length > 50)
                throw new ArgumentException("Email deve conter no máximo 50 caracteres.Tente novamente.");


            if (!(valueEmail.EndsWith("@gmail.com") ||
                  valueEmail.EndsWith("@outlook.com") ||
                  valueEmail.EndsWith("@hotmail.com") ||
                  valueEmail.EndsWith("@live.com") ||
                  valueEmail.EndsWith("@galoreceitas24horas.pt")))
            {
                throw new ArgumentException("O domínio do email inserido é inválido.Tente novamente.");
            }

            return valueEmail;
        }



        private bool ValidateUserIsAdmin(string valueUsername, string valueEmail)
        {
            if (valueUsername.Trim().ToLower().StartsWith("admin") &&
                valueEmail.Trim().ToLower().EndsWith("@galoreceitas24horas.pt"))
            {
                return true;

            }

            return false;


        }

        private string ValidatePassword(string valuePassword)
        {
            if (string.IsNullOrWhiteSpace(valuePassword))
                throw new ArgumentException("Campo obrigatório");

            if (valuePassword.Length < 8)
                throw new ArgumentException("Palavra-Passe deve conter no mínimo 8 caracteres.Tente novamente.");


            bool hasUpper = valuePassword.Any(char.IsUpper);
            bool hasLower = valuePassword.Any(char.IsLower);
            bool hasNumber = valuePassword.Any(char.IsDigit);
            bool hasSpecial = valuePassword.Any(c => !char.IsLetterOrDigit(c));

            if (!hasUpper || !hasLower || !hasNumber || !hasSpecial)
                throw new ArgumentException("Palavra-Passe deve conter uma letra maiúscula, um letra minúscula, um número e um caractere especial.Tente novamente.");

            return valuePassword;
        }


        public void ChangePassword(string newPassword)
        {
            this.Password = ValidatePassword(newPassword);
        }


        public void UpdateLockStatus(bool updateStatus)
        {


            if (this.IsLocked != updateStatus)
            {
                this.IsLocked = updateStatus;
            }


        }

        public void SetPasswordHash(string hashedPassword)
        {

            this.Password = hashedPassword;
        }


        public void IncrementFailedAttempts()
        {
            this.FailedLoginAttempts++;
        }

        public void ResetFailedAttempts()
        {
            this.FailedLoginAttempts = 0;
        }







    }
}
