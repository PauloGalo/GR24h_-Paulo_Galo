using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using GR24h_Repos;

namespace GR24h_Services
{
    public class RecipeIngredientService
    {
        private readonly RecipeIngredientRepository _riRepo;
        private readonly RecipeRepository _recipeRepo;
        private readonly IngredientRepository _ingredientRepo;
      


        public RecipeIngredientService(
            RecipeIngredientRepository riRepo,
            RecipeRepository recipeRepo,
            IngredientRepository ingredientRepo)
        {
            _riRepo = riRepo;
            _recipeRepo = recipeRepo;
            _ingredientRepo = ingredientRepo;
        }


        public void AddIngredientToRecipe(RecipeIngredient ri)
        {


            if (_recipeRepo.Retrieve(ri.RecipeId) == null)
                throw new Exception("Operação Falhada : Receita não existe.");


            List<RecipeIngredient> allCurrentRI = _riRepo.RetrieveAll();
            bool alreadyExists = allCurrentRI.Any(x => x.RecipeId == ri.RecipeId && x.IngredientId == ri.IngredientId);

            if (alreadyExists)
                throw new Exception("Operação Falhada : Ingrediente já faz parte desta receita.");


            _riRepo.Create(ri);
        }

        

        public RecipeIngredient GetRIById(int riId)
        {
            

            if (riId <= 0)
                throw new Exception("Operação Falhada : ID do ingrediente correspondente a esta receita é inválido.");

            RecipeIngredient? recipeIngredient = _riRepo.Retrieve(riId);

            if (recipeIngredient == null)
                throw new Exception("Operação Falhada : Ingrediente solicitado não existe na receita.");


            return recipeIngredient;
        }


        public List<RecipeIngredient> GetRecipeIngredientsByRecipe(int recipeId)
        {

            return _riRepo.RetrieveAll()
                .Where(ri => ri.RecipeId == recipeId)
                .ToList();
        }

       

        public List<RecipeIngredientDTO> GetRecipeIngredientsDetailsByRecipe(int recipeId)
        {
            
            List<RecipeIngredient> recipeIngredients = GetRecipeIngredientsByRecipe(recipeId);

            
            List<Ingredient> ingredients = _ingredientRepo.RetrieveAll();

            List<RecipeIngredientDTO> dtoList = new List<RecipeIngredientDTO>();

            foreach (RecipeIngredient ri in recipeIngredients)
            {
                Ingredient? ing = ingredients.FirstOrDefault(i => i.Id == ri.IngredientId);
                string ingredientName = ing != null ? ing.Name : "Ingrediente Desconhecido";

                dtoList.Add(new RecipeIngredientDTO(
                    ri.RecipeIngredientId,
                    ri.IngredientId,
                    ingredientName,
                    ri.Quantity,
                    ri.UnitMeasure
                ));
            }

            return dtoList;
        }


        public void UpdateIngredientInRecipe(RecipeIngredient ri)
        {

            if (_riRepo.Retrieve(ri.RecipeIngredientId) == null)
                throw new Exception("Operação Falhada : Ingrediente não existe na Receita.");

            _riRepo.Update(ri);

        }


        public void RemoveIngredientFromRecipe(int recipeIngredientId)
        {
            if (_riRepo.Retrieve(recipeIngredientId) == null)
                throw new Exception("Operação Falhada : Ingrediente não existe na Receita.");

            _riRepo.Delete(recipeIngredientId);
        }


        public void ClearAllIngredients(int recipeId)
        {
            List<RecipeIngredient> ingredients = GetRecipeIngredientsByRecipe(recipeId);
            foreach (RecipeIngredient item in ingredients)
            {
                _riRepo.Delete(item.RecipeIngredientId);
            }


        }

    }
}
