using GR24h_Models;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GR24h_Controller.Pages
{
    public class SearchModel : PageModel
    {
        private readonly RecipeService _recipeService;
        private readonly CategoryService _categoryService;
        private readonly DifficultyService _difficultyService;
        private readonly IngredientService _ingredientService;
        private readonly UserFavouritesService _favService;


        public SearchModel(
            RecipeService recipeService,
            CategoryService categoryService,
            DifficultyService difficultyService,
            IngredientService ingredientService,
            UserFavouritesService favService)
        {
            _recipeService = recipeService;
            _categoryService = categoryService;
            _difficultyService = difficultyService;
            _ingredientService = ingredientService;
            _favService = favService;
        }


        public List<RecipeDetailsDTO> Recipes { get; set; } = new List<RecipeDetailsDTO>();


        public List<Category> AvailableCategories { get; set; } = new List<Category>();
        public List<Difficulty> AvailableDifficulties { get; set; } = new List<Difficulty>();

        public List<Ingredient> AvailableIngredients { get; set; } = new List<Ingredient>();

        
        public List<int> UserFavouriteRecipeIds { get; set; } = new List<int>();


        [BindProperty(SupportsGet = true)]
        public RecipeFilters Filters { get; set; } = new RecipeFilters();


        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 9;

      


        public void OnGet(int id, int p = 1)
        {
            CurrentPage = p < 1 ? 1 : p;

            
            AvailableCategories = _categoryService.GetAllCategories();
            AvailableDifficulties = _difficultyService.GetAllDifficulties();
            AvailableIngredients = _ingredientService.GetAllIngredients();


            List<Recipe> filteredRecipes = _recipeService.Search(Filters, 0, int.MaxValue);

            
            int totalRecords = filteredRecipes.Count;
            TotalPages = totalRecords > 0 ? (int)Math.Ceiling((decimal)totalRecords / PageSize) : 1;

          
            List<RecipeDetailsDTO> allDetails = _recipeService.GetAllRecipesDetails();

            List<RecipeDetailsDTO> filteredDTOs = filteredRecipes
                .Select(r => allDetails.FirstOrDefault(d => d.Recipe.Id == r.Id))
                .Where(d => d != null)
                .ToList()!;

            
            int currentUserId = GetCurrentUserId();
            if (currentUserId > 0)
            {
                List<Recipe> favRecipes = _favService.GetFavouriteRecipes(currentUserId);
                if (favRecipes != null)
                {
                    UserFavouriteRecipeIds = favRecipes.Select(r => r.Id).ToList();
                }
            }
          
            int skipCount = (CurrentPage - 1) * PageSize;
            Recipes = filteredDTOs
                .Skip(skipCount)
                .Take(PageSize)
                .ToList();
        }

        private int GetCurrentUserId()
        {
           
            int? userId = HttpContext.Session.GetInt32("UserId");

           
            return userId ?? 0;
        }

        public IActionResult OnPostToggleFavourite(int recipeId)
        {
           
            int currentUserId = GetCurrentUserId();

            
            if (currentUserId <= 0)
            {
                return RedirectToPage("/LoginRegister");
            }

            try
            {
               
                bool addedToFavourites = _favService.ToggleFavourite(currentUserId, recipeId);

               
                if (addedToFavourites)
                {
                    TempData["SuccessMessage"] = "Receita adicionada aos teus favoritos!";
                }
                else
                {
                    TempData["SuccessMessage"] = "Receita removida dos teus favoritos.";
                }
            }
            catch (Exception ex)
            {
               
                TempData["ErrorMessage"] = ex.Message;
            }

            
            return RedirectToPage("/Search", new { id = recipeId });
        }

    }
}
