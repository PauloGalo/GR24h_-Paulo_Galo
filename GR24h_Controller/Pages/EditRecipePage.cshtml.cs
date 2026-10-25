using System;
using GR24h_Models;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GR24h_Controller.Pages
{
    public class EditRecipePageModel : PageModel
    {
        private readonly RecipeService _recipeService;
        private readonly CategoryService _categoryService;
        private readonly DifficultyService _difficultyService;
        private readonly IngredientService _ingredientService;
        private readonly RecipeIngredientService _riService;
        private readonly CommentService _commentService;
        private readonly RatingService _ratingService;


        public EditRecipePageModel(
            RecipeService recipeService,
            CategoryService categoryService,
            DifficultyService difficultyService,
            IngredientService ingredientService,
            RecipeIngredientService riService,
            CommentService commentService,
            RatingService ratingService)
        {
            _recipeService = recipeService;
            _categoryService = categoryService;
            _difficultyService = difficultyService;
            _ingredientService = ingredientService;
            _riService = riService;
            _commentService = commentService;
            _ratingService = ratingService;
        }

        
        public RecipeDetailsDTO RecipeDetails { get; set; } = default!;

       
        [BindProperty]
        public RecipeEditInput Input { get; set; } = new();

        public List<CommentDTO> RecipeComments { get; set; } = new();
        public List<SelectListItem> CategoryOptions { get; set; } = new();
        public List<SelectListItem> DifficultyOptions { get; set; } = new();
        public List<SelectListItem> IngredientOptions { get; set; } = new();

        [TempData]
        public string StatusMessage { get; set; } = "";

        public IActionResult OnGet(int id)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
            {
                return RedirectToPage("/LoginRegister");
            }


            RecipeDetailsDTO? details = _recipeService.GetRecipeDetails(id);
            if (details == null)
            {
                return NotFound();
            }

            RecipeDetails = details;

            Rating? userRatingObj = _ratingService.GetUserRating(id, currentUserId);
            int userRatingValue = (userRatingObj != null && userRatingObj.RatingScore.HasValue)
                ? userRatingObj.RatingScore.Value
                : 0;

            List <RecipeIngredientDTO> recipeIngredients = _riService.GetRecipeIngredientsDetailsByRecipe(id);

            List<IngredientRowInput> ingredientInputs = new List<IngredientRowInput>();

            foreach (RecipeIngredientDTO ri in recipeIngredients)
            {
                ingredientInputs.Add(new IngredientRowInput
                {
                    RecipeIngredientId = ri.RecipeIngredientId,
                    IngredientId = ri.IngredientId,
                    IngredientName = ri.IngredientName,
                    Quantity = ri.Quantity,
                    UnitMeasure = ri.UnitMeasure
                });
            }

            
            Input = new RecipeEditInput
            {
                Id = details.Recipe.Id,
                Title = details.Recipe.Title,
                CategoryId = details.Recipe.CategoryId,
                DifficultyId = details.Recipe.DifficultyId,
                UserId = details.Recipe.UserId,
                PreparationTime = details.Recipe.PreparationTime,
                PreparationMethod = details.Recipe.PreparationMethod,
                ImageUrl = details.Recipe.ImageUrl,
                UserRating = userRatingValue,
                Ingredients = ingredientInputs
            };

            RecipeComments = _commentService.GetCommentsWithUserByRecipe(id);

            
            LoadDropdownOptions();

            return Page();
        }

        public IActionResult OnPostUpdateRecipe()
        {

            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
            {
                return RedirectToPage("/LoginRegister");
            }

            
            bool isAdmin = (HttpContext.Session.GetString("UserRole") == "Admin") || User.IsInRole("Admin");
            bool isAuthor = (Input.UserId == currentUserId);

            if (!isAdmin && !isAuthor)
            {
                StatusMessage = "Não tem permissão para alterar esta receita.";
                return RedirectToPage("/Index");
            }

            ModelState.Remove("Input.Ingredients");

            if (Input.Ingredients == null || Input.Ingredients.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "A receita deve conter pelo menos um ingrediente.");
            }

            if (!ModelState.IsValid)
            {
                LoadDropdownOptions();
                RecipeComments = _commentService.GetCommentsWithUserByRecipe(Input.Id);
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
                RecipeDetailsDTO? currentDetails = _recipeService.GetRecipeDetails(Input.Id);
                decimal? existingRating = currentDetails?.Recipe.Rating;

                Category selectedCategory = _categoryService.GetCategoryById(Input.CategoryId);
                Difficulty selectedDifficulty = _difficultyService.GetDifficultyById(Input.DifficultyId);

                if (selectedCategory == null || selectedDifficulty == null)
                {
                    ModelState.AddModelError(string.Empty, "Categoria selecionada inválida ou nível de Dificuldade selecionado inválido.");
                    LoadDropdownOptions();
                    RecipeComments = _commentService.GetCommentsWithUserByRecipe(Input.Id);
                    return Page();
                }
                string currentImageUrl = !string.IsNullOrWhiteSpace(Input.ImageUrl)
                       ? Input.ImageUrl.Trim()
   :                   (currentDetails?.Recipe.ImageUrl ?? "default-recipe.jpg");

                Recipe updatedRecipeInput = new Recipe(
                    Input.Id,
                    selectedCategory.Id,
                    selectedDifficulty.Id,
                    Input.UserId,
                    Input.Title,
                    Input.PreparationMethod,
                    Input.PreparationTime,
                    existingRating,
                    currentImageUrl,
                    false,
                    null
                );

                _recipeService.UpdateRecipe(updatedRecipeInput, currentUserId,imageStream, originalFileName);

                _riService.ClearAllIngredients(Input.Id);

                foreach (IngredientRowInput row in Input.Ingredients!)
                {
                    RecipeIngredient newRi = new RecipeIngredient(
                        0,
                        Input.Id,
                        row.IngredientId,
                        row.Quantity,
                        row.UnitMeasure
                    );

                    _riService.AddIngredientToRecipe(newRi);
                }

               
                if (isAdmin)
                {
                    StatusMessage = "Receita e ingredientes atualizados e aprovados com sucesso!";
                    return RedirectToPage("/RecipePage", new { id = Input.Id });
                }
                else
                {
                    StatusMessage = "Receita atualizada com sucesso! A receita encontra-se pendente de aprovação pelo Administrador.";
                    return RedirectToPage("/PersonalAreaManagement/UserPersonalManagement/UserRecipesPending");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erro ao atualizar a receita: {ex.Message}");
                ReloadPageData(Input.Id);
                return Page();
            }
            
            finally
            {
                
                imageStream?.Dispose();
            }
        }
        public IActionResult OnPostAddIngredient(int recipeId, int ingredientId, string? newIngredientName, decimal quantity, string unitMeasure)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0) return RedirectToPage("/LoginRegister");

            try
            {
                Ingredient ing;

                
                if (!string.IsNullOrWhiteSpace(newIngredientName))
                {
                    ing = _ingredientService.GetOrCreateIngredient(newIngredientName);
                }
                else
                {
                    ing = _ingredientService.GetIngredientById(ingredientId);
                }

                if (ing != null)
                {
                    Input.Ingredients.Add(new IngredientRowInput
                    {
                        IngredientId = ing.Id,
                        IngredientName = ing.Name,
                        Quantity = quantity,
                        UnitMeasure = unitMeasure
                    });

                    StatusMessage = "Ingrediente adicionado com sucesso.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao adicionar ingrediente: {ex.Message}";
            }

            ModelState.Clear();
            ReloadPageData(Input.Id);
            return Page();
        }

      
        public IActionResult OnPostRemoveIngredient(int ingIndex)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0) return RedirectToPage("/LoginRegister");

            try
            {
                
                if (ingIndex >= 0 && ingIndex < Input.Ingredients.Count)
                {
                    Input.Ingredients.RemoveAt(ingIndex);
                    StatusMessage = "Ingrediente removido da lista.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao remover ingrediente: {ex.Message}";
            }

            ModelState.Clear();
            ReloadPageData(Input.Id);
            return Page();

        }

       
        public IActionResult OnPostClearIngredients()
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0) return RedirectToPage("/LoginRegister");

            try
            {
                Input.Ingredients.Clear();
                StatusMessage = "Todos os ingredientes foram removidos da receita.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao limpar ingredientes: {ex.Message}";
            }

            ModelState.Clear();
            ReloadPageData(Input.Id);
            return Page();
        }

        public IActionResult OnPostDeleteRecipe(int recipeId)
        {
            int currentUserId = GetCurrentUserId();

            if (currentUserId <= 0)
            {
                return RedirectToPage("/LoginRegister");
            }

            try
            {
              
                _recipeService.DeletePublishedRecipe(currentUserId, recipeId);

                
                bool isAdmin = HttpContext.Session.GetString("UserRole") == "Admin"; 

                
                if (isAdmin)
                {
                    return RedirectToPage("/PersonalAreaManagement/AdminPersonalManagement/AdminProfile", new { id = currentUserId });
                }
                else
                {
                    return RedirectToPage("/PersonalAreaManagement/UserPersonalManagement/UserProfile", new { id = currentUserId });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage("/RecipePage", new { id = recipeId });
            }
        }

        private int GetCurrentUserId()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            return userId ?? 0;
        }

        private void ReloadPageData(int recipeId)
        {
            int currentUserId = GetCurrentUserId();

            
            RecipeDetailsDTO? details = _recipeService.GetRecipeDetails(recipeId);

            if (details != null)
            {
                RecipeDetails = details;
            }

           
            Rating? userRatingObj = _ratingService.GetUserRating(recipeId, currentUserId);
            Input.UserRating = (userRatingObj != null && userRatingObj.RatingScore.HasValue)
                ? userRatingObj.RatingScore.Value
                : 0;

            
            LoadDropdownOptions();

            RecipeComments = _commentService.GetCommentsWithUserByRecipe(recipeId);
        }
        private void LoadDropdownOptions()
        {
            CategoryOptions = _categoryService.GetAllCategories()
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();

            DifficultyOptions = _difficultyService.GetAllDifficulties()
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
                .ToList();

            IngredientOptions = _ingredientService.GetAllIngredients()
                .Select(i => new SelectListItem { Value = i.Id.ToString(), Text = i.Name })
                .ToList();
        }

        #region Input Models

        public class RecipeEditInput
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public int CategoryId { get; set; }
            public int DifficultyId { get; set; }
            public int UserId { get; set; }
            public string PreparationTime { get; set; } = string.Empty;
            public string PreparationMethod { get; set; } = string.Empty;
            public string? ImageUrl { get; set; }

            public IFormFile? ImageUpload { get; set; }
            public int? UserRating { get; set; }

            public List<IngredientRowInput> Ingredients { get; set; } = new();
        }

        public class IngredientRowInput
        {
            public int RecipeIngredientId { get; set; }
            public int IngredientId { get; set; }
            public string? IngredientName { get; set; }
            public decimal Quantity { get; set; }
            public string UnitMeasure { get; set; } = string.Empty;
        }

        #endregion

    }
}
