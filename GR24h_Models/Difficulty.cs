using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR24h_Models
{
    public class Difficulty
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;

        public Difficulty(int id, string valueDifficultyName)
        {
            this.Id = id;
            this.Name = ValidateName(valueDifficultyName);
        }

        private string ValidateName(string valueDifficultyName)
        {
            if (string.IsNullOrWhiteSpace(valueDifficultyName))
                throw new ArgumentException("Campo obrigatório");

            valueDifficultyName = valueDifficultyName.Trim();


            if (valueDifficultyName.Length < 5 || valueDifficultyName.Length > 15)
                throw new ArgumentException("Dificuldade deve conter entre 5 a 15 caracteres.Tente novamente.");

            if (!valueDifficultyName.All(char.IsLetter))
                throw new ArgumentException("Dificuldade deve conter apenas letras.Tente novamente.");

            return valueDifficultyName;
        }
    }
}
