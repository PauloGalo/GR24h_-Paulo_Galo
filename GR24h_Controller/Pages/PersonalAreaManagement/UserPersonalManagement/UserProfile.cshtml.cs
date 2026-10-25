using GR24h_Models;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GR24h_Controller.Pages.PersonalAreaManagement.UserPersonalManagement
{
    public class UserProfileModel : PageModel
    {
        private readonly RecipeService _recipeService;
        private readonly UserFavouritesService _favService;
        private readonly UsersService _userService;
        private readonly RatingService _ratingService;

        public UserProfileModel(
            RecipeService recipeService,
            UserFavouritesService favService,
            UsersService userService,
            RatingService ratingService)
        {
            _recipeService = recipeService;
            _favService = favService;
            _userService = userService;
            _ratingService = ratingService;
        }


        public Users CurrentUser { get; set; } = null!;


        public List<Recipe> RecentMyRecipes { get; set; } = new List<Recipe>();
        public List<Recipe> RecentPending { get; set; } = new List<Recipe>();
        public List<Recipe> RecentFavourites { get; set; } = new List<Recipe>();


        public List<Recipe> RecentSuggestions { get; set; } = new List<Recipe>();

        public int CountApproved { get; set; }
        public int CountPending { get; set; }
        public int CountFavs { get; set; }
        public int CountSuggestions { get; set; }

        public decimal? AverageRating { get; set; }

        public Dictionary<int, decimal> RecipeRatings { get; set; } = new Dictionary<int, decimal>();

        public IActionResult OnGet(int id)
        {
            int? sessionUserId = HttpContext.Session.GetInt32("UserId");

            if (sessionUserId == null)
            {
                return RedirectToPage("/LoginRegister");
            }

            int currentUserId = sessionUserId.Value;

            Users? user = _userService.GetUserById(currentUserId);

            if (user == null)
            {
                return NotFound();
            }

            CurrentUser = user;

            AverageRating = _ratingService.GetAverageRating(id);

            RecipeRatings = _ratingService.GetAllAverageRatings();

            List<Recipe> allUserRecipes = _recipeService.ListByUser(currentUserId);
            List<Recipe> favs = _favService.GetFavouriteRecipes(currentUserId);
            List<Rating> ratings = _ratingService.GetAllRatings();
            List<Recipe> allSystemRecipes = _recipeService.Search(new RecipeFilters(), 0, int.MaxValue);

            List<int> ratedRecipeIds = ratings
           .Where(r => r.UserId == currentUserId)
           .Select(r => r.RecipeId)
           .ToList();

            List<Recipe> approved = allUserRecipes.Where(r => r.IsApproved).ToList();
            List<Recipe> pending = _recipeService.GetPendingRecipes()
                .Where(r => r.UserId == currentUserId)
                .ToList();

           
            List<Recipe> suggestions = allSystemRecipes
                .Where(r => r.UserId != currentUserId && r.IsApproved)
                .Where(r => !favs.Any(f => f.Id == r.Id))
                .Where(r => !ratedRecipeIds.Contains(r.Id))
                .ToList();

            CountApproved = approved.Count;
            CountPending = pending.Count;
            CountFavs = favs.Count;
            CountSuggestions = suggestions.Count;

            RecentMyRecipes = approved.OrderByDescending(r => r.Id).Take(5).ToList();
            RecentPending = pending.OrderByDescending(r => r.Id).Take(5).ToList();
            RecentFavourites = favs.OrderByDescending(r => r.Id).Take(5).ToList();
            RecentSuggestions = suggestions.OrderBy(x => Guid.NewGuid()).Take(5).ToList();

            return Page();
        }

       
    }
}
