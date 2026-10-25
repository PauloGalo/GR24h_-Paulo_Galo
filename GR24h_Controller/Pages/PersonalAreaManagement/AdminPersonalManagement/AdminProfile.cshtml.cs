using GR24h_Models;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GR24h_Controller.Pages.PersonalAreaManagement.AdminPersonalManagement
{
    public class AdminProfileModel : PageModel
    {
        private readonly RecipeService _recipeService;
        private readonly UsersService _userService;
        private readonly CategoryService _categoryService;
        private readonly DifficultyService _difficultyService;
        private readonly IngredientService _ingredientService;
        

        public AdminProfileModel(
            RecipeService recipeService,
            UsersService userService,
            CategoryService categoryService,
            DifficultyService difficultyService,
            IngredientService ingredientService)
        {
            _recipeService = recipeService;
            _userService = userService;
            _categoryService = categoryService;
            _difficultyService = difficultyService;
            _ingredientService = ingredientService;
          
        }


        public Users AdminUser { get; set; } = null!;
        public List<RecipeDetailsDTO> LatestPendingRecipes { get; set; } = new List<RecipeDetailsDTO>();

        public List<Users> NewestUsers { get; set; } = new List<Users>();

        public int TotalUsers { get; set; }
        public int TotalPendingRecipes { get; set; }
        public int TotalCategories { get; set; }
        public int TotalDifficulties { get; set; }
        public int TotalIngredients { get; set; }

        public IActionResult OnGet(int id)
        {
            Users? user = _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            AdminUser = user;
           
            List<RecipeDetailsDTO> allRecipesDetails = _recipeService.GetAllRecipesDetails();

           
            TotalPendingRecipes = allRecipesDetails.Count(d => !d.Recipe.IsApproved);

            List<Users> allUsers = _userService.GetAllUsers();
            TotalUsers = allUsers.Count;

            TotalCategories = _categoryService.GetAllCategories().Count;
            TotalDifficulties = _difficultyService.GetAllDifficulties().Count;
            TotalIngredients = _ingredientService.GetAllIngredients().Count;

            
            LatestPendingRecipes = allRecipesDetails
                .Where(d => !d.Recipe.IsApproved)
                .OrderByDescending(d => d.Recipe.Id)
                .Take(5)
                .ToList();

            NewestUsers = allUsers
                .OrderByDescending(u => u.Id)
                .Take(5)
                .ToList();

            return Page();
        }
    }
}
