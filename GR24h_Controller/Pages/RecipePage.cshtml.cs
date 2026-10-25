using GR24h_Models;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GR24h_Controller.Pages
{
    public class RecipePageModel : PageModel
    {
        private readonly RecipeService _recipeService;
        private readonly UserFavouritesService _favService;
        private readonly RatingService _ratingService;
        private readonly RecipeIngredientService _riService;
        private readonly CommentService _commentService;

        public RecipePageModel(
            RecipeService recipeService,
            UserFavouritesService favService,
            RatingService ratingService,
            RecipeIngredientService riService,
            CommentService commentService)
        {
            _recipeService = recipeService;
            _favService = favService;
            _ratingService = ratingService;
            _riService = riService;
            _commentService = commentService;
        }

        public Recipe CurrentRecipe { get; set; } = null!;
        public string CategoryName { get; set; } = string.Empty;
        public string DifficultyName { get; set; } = string.Empty; 
        public string AuthorUsername { get; set; } = string.Empty;


        public bool IsAuthor { get; set; }

        [BindProperty]
        public int RatingScore { get; set; }

        [BindProperty]
        public string CommentContent { get; set; } = string.Empty;

        [BindProperty]
        public int? ParentCommentId { get; set; }

        public decimal? AverageRating { get; set; }
        public bool IsFavourite { get; set; }

        public int RatingCount { get; set; }
        public int FavoriteCount { get; set; }
        public int CommentCount { get; set; }
        public decimal PopularityScore { get; set; }
        public List<RecipeIngredientDTO> Ingredients { get; set; } = new List<RecipeIngredientDTO>();

        public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();

        public List<Recipe> RelatedRecipes { get; set; } = new List<Recipe>();

        
        public IActionResult OnGet(int id)
        {
            RecipeDetailsDTO? details = _recipeService.GetRecipeDetails(id);
            if (details == null) return RedirectToPage("/Index");

            CurrentRecipe = details.Recipe;
            CategoryName = details.CategoryName;
            DifficultyName = details.DifficultyName;
            AuthorUsername = details.AuthorUsername;

            AverageRating = _ratingService.GetAverageRating(id);

          
            RatingCount = _ratingService.GetRatingCount(id);
            FavoriteCount = _favService.GetFavoriteCount(id);
            CommentCount = _commentService.GetCommentCount(id);

            PopularityScore = _recipeService.CalculatePopularityScore(
            (decimal)(AverageRating ?? 0m),
            RatingCount,
            FavoriteCount,
            CommentCount
            );

          
            int currentUserId = GetCurrentUserId();

           
            IsAuthor = (currentUserId > 0 && currentUserId == CurrentRecipe.UserId);

            if (currentUserId > 0)
            {
                IsFavourite = _favService.IsFavourite(id, currentUserId);
            }
            else
            {
                IsFavourite = false;
            }

            Ingredients = _riService.GetRecipeIngredientsDetailsByRecipe(id);
            Comments = _commentService.GetCommentsWithUserByRecipe(id);

            RelatedRecipes = _recipeService.GetRelatedRecipes(CurrentRecipe.Id, CurrentRecipe.CategoryId, CurrentRecipe.UserId, 4);

            return Page();
        }

        private int GetCurrentUserId()
        {
            
            int? userId = HttpContext.Session.GetInt32("UserId");

            return userId ?? 0;
        }

        public IActionResult OnPostAddRating(int recipeId)
        {
           
            int currentUserId = GetCurrentUserId();

            if (currentUserId <= 0)
            {
                return RedirectToPage("/LoginRegister");
            }

            if (RatingScore < 1 || RatingScore > 5)
            {
                TempData["ErrorMessage"] = "Por favor, selecione uma avaliação válida (entre 1 e 5).";
                return RedirectToPage("/RecipePage", new { id = recipeId });
            }

            try
            {
               

                Rating newRating = new Rating(0, currentUserId, recipeId, RatingScore, DateTime.Now);
             
                _ratingService.AddRating(newRating);

                TempData["SuccessMessage"] = "Obrigado por avaliar esta receita!";
            }
            catch (Exception ex)
            {
               
                TempData["ErrorMessage"] = ex.Message;
            }

            
            return RedirectToPage("/RecipePage", new { id = recipeId });
        }

        public IActionResult OnPostAddComment(int recipeId)
        {
            int currentUserId = GetCurrentUserId();

            if (currentUserId <= 0)
            {
                return RedirectToPage("/LoginRegister");
            }

            if (string.IsNullOrWhiteSpace(CommentContent))
            {
                TempData["ErrorMessage"] = "Comentário é obrigatório.";
                return RedirectToPage("/RecipePage", new { id = recipeId });
            }

            try
            {
                
                Comment newComment = new Comment(
                    0,
                    currentUserId,
                    recipeId,
                    null,
                    CommentContent,
                    DateTime.Now
                );

                _commentService.AddComment(newComment);

                TempData["SuccessMessage"] = "Comentário adicionado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToPage("/RecipePage", new { id = recipeId });
        }


        public IActionResult OnPostReplyComment(int recipeId)
        {
            int currentUserId = GetCurrentUserId();

            if (currentUserId <= 0)
            {
                return RedirectToPage("/LoginRegister");
            }

            if (!ParentCommentId.HasValue || ParentCommentId.Value <= 0)
            {
                TempData["ErrorMessage"] = "Não foi possível identificar o comentário original.";
                return RedirectToPage("/RecipePage", new { id = recipeId });
            }

            if (string.IsNullOrWhiteSpace(CommentContent))
            {
                TempData["ErrorMessage"] = "A resposta não pode estar vazia.";
                return RedirectToPage("/RecipePage", new { id = recipeId });
            }

            try
            {
                
                Comment replyComment = new Comment(
                    0,
                    currentUserId,
                    recipeId,
                    ParentCommentId.Value,
                    CommentContent,
                    DateTime.Now
                );

             
                _commentService.ReplyToComment(ParentCommentId.Value, replyComment);

                TempData["SuccessMessage"] = "Resposta enviada com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToPage("/RecipePage", new { id = recipeId });
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

            
            return RedirectToPage("/RecipePage", new { id = recipeId });
        }
    }
}
