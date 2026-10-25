using GR24h_Models;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GR24h_Controller.Pages.PersonalAreaManagement.UserPersonalManagement
{
    public class UserMyRecipesModel : PageModel
    {
        private readonly RecipeService _recipeService;

        public UserMyRecipesModel(RecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        public List<RecipeDetailsDTO> MyActiveRecipes { get; set; } = new List<RecipeDetailsDTO>();
        public int TotalApproved { get; set; }
        public int TotalPending { get; set; }

        
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

            List<RecipeDetailsDTO> allRecipesDetails = _recipeService.GetAllRecipesDetails();

            List<RecipeDetailsDTO> userRecipesDetails = allRecipesDetails
                .Where(d => d.Recipe.UserId == currentUserId.Value)
                .ToList();

            
            List<RecipeDetailsDTO> approvedRecipesList = userRecipesDetails
                .Where(d => d.Recipe.IsApproved)
                .OrderByDescending(d => d.Recipe.Id)
                .ToList();

            TotalApproved = approvedRecipesList.Count;
            TotalPending = userRecipesDetails.Count(d => !d.Recipe.IsApproved);

            TotalPages = TotalApproved > 0 ? (int)Math.Ceiling((decimal)TotalApproved / PageSize) : 1;

            
            int skipCount = (CurrentPage - 1) * PageSize;

            MyActiveRecipes = approvedRecipesList
                .Skip(skipCount)
                .Take(PageSize)
                .ToList();

            return Page();
        }
    }
}
