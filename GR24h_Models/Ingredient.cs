using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR24h_Models
{
    public class Ingredient
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;

        public Ingredient(int id, string valueIngredientName)
        {
            this.Id = id;
            this.Name = ValidateName(valueIngredientName);
        }

        private string ValidateName(string valueIngredientName)
        {
            if (string.IsNullOrWhiteSpace(valueIngredientName))
                throw new ArgumentException("Campo obrigatório");

            valueIngredientName = valueIngredientName.Trim();

            if (valueIngredientName.Length < 3 || valueIngredientName.Length > 50)
                throw new ArgumentException("Ingrediente deve conter entre 3 a 30 caracteres.Tente novamente.");

            foreach (char c in valueIngredientName)
            {
                if (!char.IsLetter(c) && !char.IsWhiteSpace(c) && c != '-')
                    throw new ArgumentException("Ingrediente deve conter apenas letras, espaços e hifens.Tente novamente.");
            }

            return valueIngredientName;
        }
    }
}
