using GR24h_Models;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GR24h_Controller.Pages.PersonalAreaManagement.UserPersonalManagement
{
    public class UserRecipesPendingModel : PageModel
    {
        private readonly RecipeService _recipeService;
       

        public UserRecipesPendingModel(RecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        public List<RecipeDetailsDTO> PendingRecipes { get; set; } = new List<RecipeDetailsDTO>();
        public int TotalCount { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 9;

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;


        public IActionResult OnGet(int p = 1)
        {
            

            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (currentUserId == null)
            {
                
                return RedirectToPage("/LoginRegister");
            }

            CurrentPage = p < 1 ? 1 : p;

            
            List<RecipeDetailsDTO> allRecipeDetails = _recipeService.GetAllRecipesDetails();

            List<RecipeDetailsDTO> allPendingRecipes = allRecipeDetails
                .Where(d => d.Recipe.UserId == currentUserId.Value && !d.Recipe.IsApproved)
                .OrderByDescending(d => d.Recipe.Id)
                .ToList();


           
            TotalCount = allPendingRecipes.Count;

        
            TotalPages = TotalCount > 0 ? (int)Math.Ceiling((decimal) TotalCount/ PageSize) : 1;

      
            int skipCount = (CurrentPage - 1) * PageSize;

            PendingRecipes = allPendingRecipes
                .Skip(skipCount)
                .Take(PageSize)
                .ToList();


            return Page();

        }
    }
}
