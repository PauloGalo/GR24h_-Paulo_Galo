using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using GR24h_Repos;

namespace GR24h_Services
{
    public class IngredientService
    {
        private readonly IngredientRepository _ingredientRepo;
        private readonly RecipeIngredientRepository _riRepo;

        public IngredientService(IngredientRepository ingredientRepo, RecipeIngredientRepository riRepo)
        {
            _ingredientRepo = ingredientRepo;
            _riRepo = riRepo;
        }

        public void CreateIngredientByAdmin(Ingredient newIngredient)
        {
            bool alreadyExists = _ingredientRepo.RetrieveAll()
                .Any(i => i.Name.ToLower() == newIngredient.Name.ToLower());

            if (alreadyExists)
            {
                throw new Exception($"Nome do Ingrediente '{newIngredient.Name}' já existe. Tente novamente.");
            }

            _ingredientRepo.Create(newIngredient);
        }

        public Ingredient GetOrCreateIngredient(string name)
        {
            List<Ingredient> allIngredients = _ingredientRepo.RetrieveAll();

            Ingredient? existing = allIngredients.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                return existing;
            }


            Ingredient newIngredient = new Ingredient(0, name);
            return _ingredientRepo.Create(newIngredient);
        }

        public Ingredient GetIngredientById(int ingId)
        {
            if (ingId <= 0)
                throw new Exception("Operação Falhada : ID de Ingrediente inválido.");

            Ingredient? ingredient = _ingredientRepo.Retrieve(ingId);

            if (ingredient == null)
                throw new Exception("Operação Falhada : Ingrediente solicitado não existe.");

            return ingredient;
        }


        public List<Ingredient> GetAllIngredients()
        {
            List<Ingredient> ingredients = _ingredientRepo.RetrieveAll();

            if (ingredients == null)
            {
                return new List<Ingredient>();
            }

            return ingredients
                .OrderBy(i => i.Name)
                .ToList();
        }

        public void UpdateIngredientByAdmin(Ingredient updatedIngredient)
        {
            if (updatedIngredient.Id <= 0)
                throw new Exception("Operação Falhada : ID de Ingrediente inválido.");


            bool alreadyExists = _ingredientRepo.RetrieveAll()
                .Any(i => i.Name.ToLower() == updatedIngredient.Name.ToLower() && i.Id != updatedIngredient.Id);

            if (alreadyExists)
                throw new Exception($"Nome do Ingrediente '{updatedIngredient.Name}' já existe. Tente novamente.");

            _ingredientRepo.Update(updatedIngredient);
        }

        public void DeleteIngredientByAdmin(int id)
        {
            if (id <= 0)
                throw new Exception("Operação Falhada : ID de Ingrediente inválido.");

            bool hasRecipes = _riRepo.RetrieveAll().Any(r => r.IngredientId == id);

            if (hasRecipes)
                throw new Exception("Operação Negada: Existem receitas associadas a este ingrediente.");

            _ingredientRepo.Delete(id);
        }
    }
}
