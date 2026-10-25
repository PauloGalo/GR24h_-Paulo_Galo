using GR24h_Models;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GR24h_Controller.Pages.PersonalAreaManagement.UserPersonalManagement
{
    public class UserCreateRecipeModel : PageModel
    {
        private readonly RecipeService _recipeService;
        private readonly RecipeIngredientService _riService;
        private readonly CategoryService _categoryService;
        private readonly DifficultyService _difficultyService;
        private readonly IngredientService _ingredientService;
        private readonly RatingService _ratingService;
        private readonly CommentService _commentService;

        public UserCreateRecipeModel(
            RecipeService recipeService,
            RecipeIngredientService riService,
            CategoryService categoryService,
            DifficultyService difficultyService,
            IngredientService ingredientService,          
            RatingService ratingService,
            CommentService commentService)
        {
            _recipeService = recipeService;
            _riService = riService;
            _categoryService = categoryService;
            _difficultyService = difficultyService;
            _ingredientService = ingredientService;
            _ratingService = ratingService;
            _commentService = commentService;
        }

        public List<SelectListItem> CategoryOptions { get; set; } = new();
        public List<SelectListItem> DifficultyOptions { get; set; } = new();
        public List<SelectListItem> IngredientOptions { get; set; } = new();

        [BindProperty]
        public RecipeCreateInput Input { get; set; } = new();

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            LoadViewData();
        }

        public IActionResult OnPost()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");


            if (currentUserId != null)
            {
                Input.UserId = currentUserId.Value;

                ModelState.Remove("Input.Ingredients");

                if (!ModelState.IsValid)
                {
                    LoadViewData();
                    return Page();
                }

                Stream? imageStream = null;
                string? originalFileName = null;

                if (Input.ImageUpload != null && Input.ImageUpload.Length > 0)
                {
                    imageStream = Input.ImageUpload.OpenReadStream();
                    originalFileName = Input.ImageUpload.FileName;
                }

                try
                {

                    Category selectedCategory = _categoryService.GetCategoryById(Input.CategoryId);
                    Difficulty selectedDifficulty = _difficultyService.GetDifficultyById(Input.DifficultyId);

                    if (selectedCategory == null || selectedDifficulty == null)
                    {
                        ModelState.AddModelError(string.Empty, "Categoria selecionada inválida ou nível de Dificuldade selecionado inválido.");
                        LoadViewData();
                        return Page();
                    }

                    if (!string.IsNullOrWhiteSpace(Input.ImageUrl))
                    {
                        Input.ImageUrl = Input.ImageUrl.Trim();
                    }
                    else
                    {
                        Input.ImageUrl = "default-recipe.jpg";
                    }

                    Recipe newRecipe = new Recipe(
                        0,
                        selectedCategory.Id,
                        selectedDifficulty.Id,
                        Input.UserId,
                        Input.Title,
                        Input.PreparationMethod,
                        Input.PreparationTime,
                        Input.InitialRatingScore,
                        Input.ImageUrl ?? "default-recipe.jpg",
                        false,
                        null

                    );

                    
                    Recipe savedRecipe = _recipeService.CreateRecipe(newRecipe, imageStream, originalFileName);


                    if (Input.InitialRatingScore.HasValue && Input.InitialRatingScore.Value > 0)
                    {
                        Rating initialRating = new Rating(
                            0,
                            Input.UserId,
                            savedRecipe.Id,
                            (int)Input.InitialRatingScore.Value,
                            DateTime.Now
                        );
                        _ratingService.AddRating(initialRating);
                    }


                    if (!string.IsNullOrWhiteSpace(Input.InitialCommentContent))
                    {
                        Comment initialComment = new Comment(
                            0,
                            Input.UserId,
                            savedRecipe.Id,
                            null,
                            Input.InitialCommentContent,
                            DateTime.Now);
                        _commentService.AddComment(initialComment);
                    }


                    if (Input.Ingredients != null && Input.Ingredients.Any())
                    {
                        foreach (IngredientRowInput item in Input.Ingredients)
                        {
                            Ingredient ing;
                            if (!string.IsNullOrWhiteSpace(item.IngredientName))
                            {
                                ing = _ingredientService.GetOrCreateIngredient(item.IngredientName);
                            }
                            else
                            {
                                ing = _ingredientService.GetIngredientById(item.IngredientId);
                            }

                            if (ing != null)
                            {
                                RecipeIngredient ri = new RecipeIngredient(0, savedRecipe.Id, ing.Id, item.Quantity, item.UnitMeasure);
                                _riService.AddIngredientToRecipe(ri);
                            }
                        }
                    }

                    StatusMessage = "Receita enviada com sucesso! Ficará disponível assim que for aprovada pelo administrador.";
                    return RedirectToPage("/PersonalAreaManagement/UserPersonalManagement/UserRecipesPending");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Erro interno: {ex.Message}");
                    LoadViewData();
                    return Page();
                }

                finally
                {
                    
                    imageStream?.Dispose();
                }
            }

            else
            {
                return RedirectToPage("Login/Register");
            }
        }

        private void LoadViewData()
        {
            CategoryOptions = _categoryService.GetAllCategories().Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList();
            DifficultyOptions = _difficultyService.GetAllDifficulties().Select(d => new SelectListItem(d.Name, d.Id.ToString())).ToList();
            IngredientOptions = _ingredientService.GetAllIngredients()
        .   OrderBy(i => i.Name)
        .   Select(i => new SelectListItem
        {
            Text = i.Name,
            Value = i.Id.ToString()
        })
        .
           ToList();
        }

     
        #region DTOs

        public class RecipeCreateInput
        {
            public string Title { get; set; } = string.Empty;
            public int CategoryId { get; set; }
            public int DifficultyId { get; set; }

            public int UserId { get; set; }

            public string PreparationTime { get; set; } = string.Empty;
            public string PreparationMethod { get; set; } = string.Empty;
            public string? ImageUrl { get; set; }

            public IFormFile? ImageUpload { get; set; }
            public decimal? InitialRatingScore { get; set; }
            public string? InitialCommentContent { get; set; } = string.Empty;

            public List<IngredientRowInput> Ingredients { get; set; } = new();
        }

        public class IngredientRowInput
        {
            public int IngredientId { get; set; }
            public string? IngredientName { get; set; }
            public decimal Quantity { get; set; }
            public string UnitMeasure { get; set; } = string.Empty;
        }

        #endregion
    }
}
