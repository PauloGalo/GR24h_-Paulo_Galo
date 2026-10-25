using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR24h_Models
{
    public class RecipeIngredientDTO
    {
        public int RecipeIngredientId { get; set; }
        public int IngredientId { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string UnitMeasure { get; set; } = string.Empty;

        public RecipeIngredientDTO(int recipeIngredientId, int ingredientId, string ingredientName, decimal quantity, string unitMeasure)
        {
            RecipeIngredientId = recipeIngredientId;
            IngredientId = ingredientId;
            IngredientName = ingredientName;
            Quantity = quantity;
            UnitMeasure = unitMeasure;
        }
    }



    public class RecipeIngredient
    {
        public int RecipeIngredientId { get; private set; }
        public int RecipeId { get; private set; }
        public int IngredientId { get; private set; }


      

        public decimal Quantity { get; private set; }
        public string UnitMeasure { get; private set; } = null!;




        public RecipeIngredient(
            int recipeIngredientId,
            int recipeId,
            int ingredientId,
            decimal valueRecipeIngredientQuantity,
            string valueRecipeIngredientUnitMeasure)
        {

            this.RecipeIngredientId = recipeIngredientId;
            this.RecipeId = recipeId;
            this.IngredientId = ingredientId;

            this.Quantity = RecipeIngredientQuantity(valueRecipeIngredientQuantity);
            this.UnitMeasure = RecipeIngredientUnitMeasure(valueRecipeIngredientUnitMeasure);

        }

        public decimal RecipeIngredientQuantity(decimal valueRecipeIngredientQuantity)
        {

            if (valueRecipeIngredientQuantity <= 0)
                throw new ArgumentException("Campo obrigatório e superior a 0. Tente novamente.");

            return valueRecipeIngredientQuantity;
        }

        public string RecipeIngredientUnitMeasure(string valueRecipeIngredientUnitMeasure)
        {
            if (string.IsNullOrWhiteSpace(valueRecipeIngredientUnitMeasure))
                throw new ArgumentException("Campo obrigatório");

            valueRecipeIngredientUnitMeasure = valueRecipeIngredientUnitMeasure.Trim();

            if (valueRecipeIngredientUnitMeasure.Length > 20)
                throw new ArgumentException("Unidade de Medida deve conter no máximo 20 caracteres.Tente novamente.");

            if (valueRecipeIngredientUnitMeasure.Any(c => !char.IsLetter(c) && !char.IsWhiteSpace(c) && c != '-' && c != '.'))
                throw new ArgumentException("Unidade de Medida deve conter apenas letras, espaços, hifens e pontos. Tente novamente.");

            return valueRecipeIngredientUnitMeasure;
        }


    }
}
