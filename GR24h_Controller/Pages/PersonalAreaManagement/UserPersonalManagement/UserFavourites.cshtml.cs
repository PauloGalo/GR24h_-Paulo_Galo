using GR24h_Models;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GR24h_Controller.Pages.PersonalAreaManagement.UserPersonalManagement
{
    public class UserFavouritesModel : PageModel
    {
        private readonly UserFavouritesService _favService;
        private readonly RecipeService _recipeService;

        public UserFavouritesModel(
            UserFavouritesService favService,
            RecipeService recipeService)
        {
            _favService = favService;
            _recipeService = recipeService;
        }

     
        public List<RecipeDetailsDTO> FavouriteRecipes { get; set; } = new List<RecipeDetailsDTO>();
        public int TotalCount { get; set; }

        
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 9;


        public IActionResult OnGet(int p = 1)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (currentUserId == null)
            {
                return RedirectToPage("/LoginRegister");
            }

            CurrentPage = p < 1 ? 1 : p;

            
            List<Recipe> favRecipes = _favService.GetFavouriteRecipes(currentUserId.Value);
            List<int> favIds = favRecipes.Select(f => f.Id).ToList();

            List<RecipeDetailsDTO> allDetails = _recipeService.GetAllRecipesDetails();

            List<RecipeDetailsDTO> allFavouriteDTOs = allDetails
           .Where(d => favIds.Contains(d.Recipe.Id))
           .OrderByDescending(d => d.Recipe.Id)
           .ToList();

            TotalCount = allFavouriteDTOs.Count;

           
            TotalPages = TotalCount > 0 ? (int)Math.Ceiling((decimal)TotalCount / PageSize) : 1;

           
            int skipCount = (CurrentPage - 1) * PageSize;

            FavouriteRecipes = allFavouriteDTOs
                .Skip(skipCount)
                .Take(PageSize)
                .ToList();

            return Page();
        }

        public IActionResult OnPostRemove(int recipeId)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (currentUserId == null)
            {
                return RedirectToPage("/LoginRegister");
            }

            try
            {
                _favService.ToggleFavourite(currentUserId.Value, recipeId);
                return RedirectToPage();
            }
            catch (System.Exception)
            {
                return Page();
            }
        }
    }
}
