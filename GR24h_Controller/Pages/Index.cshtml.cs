using GR24h_Models;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GR24h_Controller.Pages
{
    public class IndexModel : PageModel
    {
        

        private readonly RecipeService _recipeService;
      


        public IndexModel(RecipeService recipeService)
        {
            _recipeService = recipeService;
            
        }
        public List<RecipeDetailsDTO> TopRatedRecipes { get; set; } = new List<RecipeDetailsDTO>();
        public List<RecipeDetailsDTO> PopularRecipes { get; set; } = new List<RecipeDetailsDTO>();
        public List<RecipeDetailsDTO> NewestRecipes { get; set; } = new List<RecipeDetailsDTO>();

       
        
        public void OnGet()
        {
            List<Recipe> topRated = _recipeService.GetTopRatedRecipes(6);
            TopRatedRecipes = topRated
                .Select(r => _recipeService.GetRecipeDetails(r.Id))
                .Where(dto => dto != null)
                .ToList()!;

            List<Recipe> popular = _recipeService.GetMostPopularRecipes(6);
            PopularRecipes = popular
                .Select(r => _recipeService.GetRecipeDetails(r.Id))
                .Where(dto => dto != null)
                .ToList()!;

            List<Recipe> newest = _recipeService.GetNewestRecipes(6);
            NewestRecipes = newest
                .Select(r => _recipeService.GetRecipeDetails(r.Id))
                .Where(dto => dto != null)
                .ToList()!;

        }
    }
}
