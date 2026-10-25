using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using GR24h_Repos;

namespace GR24h_Services
{
    public class CategoryService
    {
        private readonly CategoryRepository _categoryRepo;
        private readonly RecipeService _recipeService;

        public CategoryService(CategoryRepository categoryRepo, RecipeService recipeService)
        {
            _categoryRepo = categoryRepo;
            _recipeService = recipeService;
        }


        public void CreateCategoryByAdmin(Category newCategory)
        {
            bool alreadyExists = _categoryRepo.RetrieveAll()
                .Any(c => c.Name.ToLower() == newCategory.Name.ToLower());

            if (alreadyExists)
                throw new Exception($"A Categoria '{newCategory.Name}' já existe.Tente novamente.");

            _categoryRepo.Create(newCategory);
        }


        public Category GetCategoryById(int catId)
        {
            if (catId <= 0)
                throw new Exception("Operação Falhada : ID de Categoria inválido.");

            Category? category = _categoryRepo.Retrieve(catId);

            if (category == null)
                throw new Exception("Operação Falhada : Categoria solicitada não existe.");

            return category;
        }


        public List<Category> GetAllCategories()
        {
            List<Category> categories = _categoryRepo.RetrieveAll();

            if (categories == null)
            {
                return new List<Category>();
            }

            return categories
                .OrderBy(c => c.Name)
                .ToList();
        }

        public void UpdateCategoryByAdmin(Category updatedCategory)
        {
            if (updatedCategory.Id <= 0)
                throw new Exception("Operação Falhada : ID de Categoria inválido.");

            bool alreadyExists = _categoryRepo.RetrieveAll()
                .Any(c => c.Name.ToLower() == updatedCategory.Name.ToLower() && c.Id != updatedCategory.Id);

            if (alreadyExists)
                throw new Exception($"A Categoria '{updatedCategory.Name}' já existe.Tente novamente.");

            _categoryRepo.Update(updatedCategory);
        }

        public void DeleteCategoryByAdmin(int id)
        {
            if (id <= 0)
                throw new Exception("Operação Falhada : ID de Categoria inválido.");

            bool hasRecipes = _recipeService.GetAllRecipesDetails().Any(r => r.Recipe.CategoryId == id);

            if (hasRecipes)
                throw new Exception("Operação Negada: Existem receitas associadas a esta categoria.");

            _categoryRepo.Delete(id);
        }
    }
}
