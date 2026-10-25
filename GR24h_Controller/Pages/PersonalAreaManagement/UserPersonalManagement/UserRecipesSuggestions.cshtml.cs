using GR24h_Models;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GR24h_Controller.Pages.PersonalAreaManagement.UserPersonalManagement
{
    public class UserRecipesSuggestionsModel : PageModel
    {
        private readonly RecipeService _recipeService;
        private readonly UserFavouritesService _favService;
        private readonly RatingService _ratingService;


        public UserRecipesSuggestionsModel(
            RecipeService recipeService,
            UserFavouritesService favService,
            RatingService ratingService)
        {
            _recipeService = recipeService;
            _favService = favService;
            _ratingService = ratingService;
        }

        public List<RecipeDetailsDTO> SuggestedRecipes { get; set; } = new List<RecipeDetailsDTO>();
        public int TotalCount { get; set; }

       
        public IActionResult OnGet()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (currentUserId == null)
            {
                return RedirectToPage("/LoginRegister");
            }

            List<RecipeDetailsDTO> allRecipesDetails = _recipeService.GetAllRecipesDetails();

            List<int> favIds = _favService.GetFavouriteRecipes(currentUserId.Value)
                .Select(f => f.Id)
                .ToList();

            List<int> ratedIds = _ratingService.GetAllRatings()
                .Where(r => r.UserId == currentUserId.Value)
                .Select(r => r.RecipeId)
                .ToList();

            
            List<RecipeDetailsDTO> filteredList = allRecipesDetails
                .Where(d => d.Recipe.IsApproved
                         && d.Recipe.UserId != currentUserId.Value
                         && !favIds.Contains(d.Recipe.Id)
                         && !ratedIds.Contains(d.Recipe.Id))
                .ToList();

            TotalCount = filteredList.Count;

           
            SuggestedRecipes = filteredList
                .OrderBy(d => Guid.NewGuid())
                .Take(6)
                .ToList();

            return Page();
        }
    }
}
