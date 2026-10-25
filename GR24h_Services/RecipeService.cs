using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using GR24h_Repos;

namespace GR24h_Services
{

    public class RecipeService
    {
        private readonly RecipeRepository _recipeRepo;
        private readonly CategoryRepository _catRepo;
        private readonly DifficultyRepository _diffRepo;
        private readonly UsersRepository _userRepo;
        private readonly IngredientRepository _ingredientRepo;
        private readonly RecipeIngredientRepository _riRepo;
        private readonly RecipeIngredientService _riService;
        private readonly RatingRepository _ratingRepo;
        private readonly UserFavouritesRepository _userFavRepo;
        private readonly RatingService _ratingService;
        private readonly CommentService _commentService;
        private readonly FileService _fileService;

        public RecipeService(
                RecipeRepository recipeRepo,
                CategoryRepository catRepo,
                DifficultyRepository diffRepo,
                UsersRepository userRepo,
                IngredientRepository ingredientRepo,
                RecipeIngredientRepository riRepo,
                RatingRepository ratingRepo,
                UserFavouritesRepository userFavRepo,
                RecipeIngredientService riService,
                RatingService ratingService,
                CommentService commentService,
                FileService fileService)
        {
            _recipeRepo = recipeRepo;
            _catRepo = catRepo;
            _diffRepo = diffRepo;
            _userRepo = userRepo;
            _ingredientRepo = ingredientRepo;
            _riRepo = riRepo;
            _ratingRepo = ratingRepo;
            _userFavRepo = userFavRepo;
            _riService = riService;
            _ratingService = ratingService;
            _commentService = commentService;
            _fileService = fileService;
        }



        public Recipe CreateRecipe(Recipe newRecipe, Stream? imageStream, string? originalFileName)
        {
            

            if (_catRepo.Retrieve(newRecipe.CategoryId) == null)
                throw new Exception("Operação Falhada : Categoria selecionada não existe.");

            if (_diffRepo.Retrieve(newRecipe.DifficultyId) == null)
                throw new Exception("Operação Falhada : Nível de Dificuldade selecionado não existe.");

            if (_userRepo.Retrieve(newRecipe.UserId) == null)
                throw new Exception("Operação Falhada : Utilizador não existe.");

            //bool recipeTitleExists = _recipeRepo.RetrieveAll().Any(r =>
            //    r.Title.Equals(newRecipe.Title, StringComparison.OrdinalIgnoreCase));

            //if (recipeTitleExists)
            //{
            //    throw new Exception("Operação Falhada : Nome da Receita inserido já existe.Tente novamente.");
            //}

            string? imageToValidate = null;

            
            if (imageStream != null && !string.IsNullOrEmpty(originalFileName))
            {
                imageToValidate = _fileService.SaveImage(imageStream, originalFileName, "images/recipes");
            }

            else if (!string.IsNullOrWhiteSpace(newRecipe.ImageUrl))
            {
                imageToValidate = newRecipe.ImageUrl.Trim();
            }

            
            string finalImageUrl = newRecipe.RecipeImage(imageToValidate);

            
            Recipe recipeToSave = new Recipe(
                newRecipe.Id,
                newRecipe.CategoryId,
                newRecipe.DifficultyId,
                newRecipe.UserId,
                newRecipe.Title,
                newRecipe.PreparationMethod,
                newRecipe.PreparationTime,
                newRecipe.Rating,
                finalImageUrl,
                newRecipe.IsApproved,
                null
            );

            return _recipeRepo.Create(recipeToSave);

           
        }

        public Recipe GetRecipeById(int recipeId)
        {

            if (recipeId <= 0)
                throw new Exception("Operação Falhada : ID de Receita inválido.");

            Recipe? recipe = _recipeRepo.Retrieve(recipeId);

            if (recipe == null)
                throw new Exception("Operação Falhada : Receita solicitada não existe.");

            return recipe;
        }

        public RecipeDetailsDTO? GetRecipeDetails(int id)
        {
          
            Recipe? recipe = _recipeRepo.Retrieve(id);
            if (recipe == null) return null;

            decimal? averageRating = _ratingService.GetAverageRating(id);

            recipe.UpdateAverageRating(averageRating);

            
            Category? cat = _catRepo.RetrieveAll().FirstOrDefault(c => c.Id == recipe.CategoryId);
            string categoryName = cat != null ? cat.Name : "Sem Categoria";

            
            Difficulty? diff = _diffRepo.RetrieveAll().FirstOrDefault(d => d.Id == recipe.DifficultyId);
            string difficultyName = diff != null ? diff.Name : "Nível de Dificuldade não especificado";

            
            Users? author = _userRepo.RetrieveAll().FirstOrDefault(u => u.Id == recipe.UserId);
            string authorUsername = author != null ? author.Username : "Autor Desconhecido";

            int rCount = _ratingService.GetAllRatingCounts().GetValueOrDefault(id, 0);
            int fCount = _userFavRepo.RetrieveAll().Count(f => f.RecipeId == id);
            int cCount = _commentService.GetAllCommentCounts().GetValueOrDefault(id, 0);
            decimal avg = (decimal)(averageRating ?? 0m);

            decimal popularityScore = CalculatePopularityScore(avg, rCount, fCount, cCount);

            
            return new RecipeDetailsDTO(recipe, categoryName, difficultyName, authorUsername, popularityScore);


        }


        public List<RecipeDetailsDTO> GetAllRecipesDetails()
        {
            List<Recipe> recipes = _recipeRepo.RetrieveAll();
            List<Category> categories = _catRepo.RetrieveAll();
            List<Difficulty> difficulties = _diffRepo.RetrieveAll();
            List<Users> users = _userRepo.RetrieveAll();

            Dictionary<int, int> ratingCounts = _ratingService.GetAllRatingCounts();
            Dictionary<int, int> commentCounts = _commentService.GetAllCommentCounts();
            Dictionary<int, decimal> averageRatings = _ratingService.GetAllAverageRatings();
            List<UserFavourites> allFavorites = _userFavRepo.RetrieveAll();

            List<RecipeDetailsDTO> dtoList = new List<RecipeDetailsDTO>();

            foreach (Recipe recipe in recipes)
            {
                decimal? averageRating = _ratingService.GetAverageRating(recipe.Id);
                recipe.UpdateAverageRating(averageRating);

                Category? cat = categories.FirstOrDefault(c => c.Id == recipe.CategoryId);
                string categoryName = cat != null ? cat.Name : "Sem Categoria";

                Difficulty? diff = difficulties.FirstOrDefault(d => d.Id == recipe.DifficultyId);
                string difficultyName = diff != null ? diff.Name : "Nível de Dificuldade não especificado";

                Users? author = users.FirstOrDefault(u => u.Id == recipe.UserId);
                string authorUsername = author != null ? author.Username : "Autor Desconhecido";

                int rCount = ratingCounts.GetValueOrDefault(recipe.Id, 0);
                int fCount = allFavorites.Count(f => f.RecipeId == recipe.Id);
                int cCount = commentCounts.GetValueOrDefault(recipe.Id, 0);
                decimal avg = averageRatings.GetValueOrDefault(recipe.Id, 0.0m);

                
                decimal popularityScore = CalculatePopularityScore(avg, rCount, fCount, cCount);

                dtoList.Add(new RecipeDetailsDTO(recipe, categoryName, difficultyName, authorUsername, popularityScore));
            }

            return dtoList;
        }

        public List<RecipeDetailsDTO> GetPendingRecipesDetails()
        {
            List<Recipe> recipes = GetPendingRecipes(); 
            List<Category> categories = _catRepo.RetrieveAll();
            List<Difficulty> difficulties = _diffRepo.RetrieveAll();
            List<Users> users = _userRepo.RetrieveAll();

            Dictionary<int, int> ratingCounts = _ratingService.GetAllRatingCounts();
            Dictionary<int, int> commentCounts = _commentService.GetAllCommentCounts();
            Dictionary<int, decimal> averageRatings = _ratingService.GetAllAverageRatings();
            List<UserFavourites> allFavorites = _userFavRepo.RetrieveAll();

            List<RecipeDetailsDTO> dtoList = new List<RecipeDetailsDTO>();

            foreach (Recipe recipe in recipes)
            {
                decimal? averageRating = _ratingService.GetAverageRating(recipe.Id);
                recipe.UpdateAverageRating(averageRating);

                Category? cat = categories.FirstOrDefault(c => c.Id == recipe.CategoryId);
                string categoryName = cat != null ? cat.Name : "Sem Categoria";

                Difficulty? diff = difficulties.FirstOrDefault(d => d.Id == recipe.DifficultyId);
                string difficultyName = diff != null ? diff.Name : "Nível de Dificuldade não especificado";

                Users? author = users.FirstOrDefault(u => u.Id == recipe.UserId);
                string authorUsername = author != null ? author.Username : "Autor Desconhecido";

                int rCount = ratingCounts.GetValueOrDefault(recipe.Id, 0);
                int fCount = allFavorites.Count(f => f.RecipeId == recipe.Id);
                int cCount = commentCounts.GetValueOrDefault(recipe.Id, 0);
                decimal avg = averageRatings.GetValueOrDefault(recipe.Id, 0.0m);

                decimal popularityScore = CalculatePopularityScore(avg, rCount, fCount, cCount);

                dtoList.Add(new RecipeDetailsDTO(recipe, categoryName, difficultyName, authorUsername, popularityScore));
            }

            return dtoList;
        }

        public List<Recipe> GetPendingRecipes()
        {

            return _recipeRepo.RetrieveAll()
                .Where(r => !r.IsApproved)
                .ToList();
        }

        public List<Recipe> ListByCategory(int categoryId)
        {
            if (_catRepo.Retrieve(categoryId) == null)
                throw new Exception("Operação Falhada : Categoria não existe.");

            return _recipeRepo.RetrieveAll()
                .Where(r => r.CategoryId == categoryId && r.IsApproved)
                .ToList();
        }


        public List<Recipe> ListByDifficulty(int difficultyId)
        {

            if (_diffRepo.Retrieve(difficultyId) == null)
                throw new Exception("Operação Falhada : Nível de Dificuldade não existe.");


            return _recipeRepo.RetrieveAll()
                .Where(r => r.DifficultyId == difficultyId && r.IsApproved)
                .ToList();
        }


        public List<Recipe> ListByUser(int userId)
        {
            if (_userRepo.Retrieve(userId) == null)
                throw new Exception("Operação Falhada : Utilizador não existe.");

            return _recipeRepo.RetrieveAll()
                .Where(r => r.UserId == userId && r.IsApproved)
                .ToList();
        }


        public List<Recipe> Search(RecipeFilters filters, int skip = 0, int take = 9)
        {

            IEnumerable<Recipe> recipes = _recipeRepo.RetrieveAll().Where(r => r.IsApproved);

            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
            {
                string term = filters.SearchTerm.Trim();
                string cleanTerm = RemoveAccents(term);

                List<int> userIds = _userRepo.RetrieveAll()
                    .Where(u => RemoveAccents (u.Username).Contains(cleanTerm, StringComparison.OrdinalIgnoreCase))
                    .Select(u => u.Id).ToList();

                List<int> catIds = _catRepo.RetrieveAll()
                    .Where(c => RemoveAccents (c.Name).Contains(cleanTerm, StringComparison.OrdinalIgnoreCase))
                    .Select(c => c.Id).ToList();

                List<int> diffIds = _diffRepo.RetrieveAll()
                    .Where(d => RemoveAccents (d.Name).Contains(cleanTerm, StringComparison.OrdinalIgnoreCase))
                    .Select(d => d.Id).ToList();

                List<int> ingIds = _ingredientRepo.RetrieveAll()
                    .Where(i => RemoveAccents (i.Name).Contains(cleanTerm, StringComparison.OrdinalIgnoreCase))
                    .Select(i => i.Id).ToList();

                List<int> recipeIdsFromIngredients = _riRepo.RetrieveAll()
                    .Where(ri => ingIds.Contains(ri.IngredientId))
                    .Select(ri => ri.RecipeId).ToList();

                recipes = recipes.Where(r =>
                    RemoveAccents(r.Title).Contains(cleanTerm, StringComparison.OrdinalIgnoreCase) ||
                    userIds.Contains(r.UserId) ||
                    catIds.Contains(r.CategoryId) ||
                    diffIds.Contains(r.DifficultyId) ||
                    recipeIdsFromIngredients.Contains(r.Id)
                );
            }

           
            if (!string.IsNullOrWhiteSpace(filters.Author))
            {
                List<int> filterAuthor = _userRepo.RetrieveAll()
                    .Where(u => RemoveAccents(u.Username).Contains(RemoveAccents(filters.Author), StringComparison.OrdinalIgnoreCase))
                    .Select(u => u.Id)
                    .ToList();

                recipes = recipes.Where(r => filterAuthor.Contains(r.UserId));
            }

            if (!string.IsNullOrWhiteSpace(filters.Name))
            {
                recipes = recipes.Where(r => RemoveAccents(r.Title).Contains(RemoveAccents(filters.Name), StringComparison.OrdinalIgnoreCase));
            }


            if (filters.Categories != null && filters.Categories.Any())
            {
               
                List<string> filterCat = filters.Categories.Select(f => RemoveAccents(f).ToLower()).ToList();

               
                List<int> categoryIds = _catRepo.RetrieveAll()
                    .Where(c => filterCat.Contains(RemoveAccents(c.Name).ToLower()))
                    .Select(c => c.Id)
                    .ToList();

                recipes = recipes.Where(r => categoryIds.Contains(r.CategoryId));
            }


            if (filters.Difficulties != null && filters.Difficulties.Any())
            {
                List<string> filterDiff = filters.Difficulties.Select(f => RemoveAccents(f).ToLower()).ToList();

               
                List<int> difficultyIds = _diffRepo.RetrieveAll()
                    .Where(d => filterDiff.Contains(RemoveAccents(d.Name).ToLower()))
                    .Select(d => d.Id)
                    .ToList();

                
                recipes = recipes.Where(r => difficultyIds.Contains(r.DifficultyId));


            }


            if (filters.Ingredients != null && filters.Ingredients.Any())
            {
                List<string> filterIng = filters.Ingredients
                    .Where(i => !string.IsNullOrWhiteSpace(i))
                    .Select(f => RemoveAccents(f))
                    .ToList();


                if(filterIng.Any())
                {
                    List<int> ingredientIds = _ingredientRepo.RetrieveAll()
                        .Where(i => filterIng.Contains(RemoveAccents(i.Name).ToLower()))
                        .Select(i => i.Id)
                        .ToList();

                    List<int> recipeIds = _riRepo.RetrieveAll()
                        .Where(ri => ingredientIds.Contains(ri.IngredientId))
                        .Select(ri => ri.RecipeId)
                        .Distinct()
                        .ToList();

                    recipes = recipes.Where(r => recipeIds.Contains(r.Id));

                }  
                
            }

            switch (filters.SortBy?.ToLower())
            {
                case "title_asc":
                case "az":
                case "a-z":
                case "alfabetica":
                    recipes = recipes
                        .OrderBy(r => r.Title)
                        .ToList();
                    break;

                case "rating_desc":
                case "rating":
                case "avaliadas":
                case "mais_avaliadas":
                    List<Recipe> topRated = GetTopRatedRecipes(int.MaxValue);
                    recipes = recipes
                        .OrderBy(r => topRated.FindIndex(t => t.Id == r.Id))
                        .ToList();
                    break;

                case "popular":
                case "populares":
                case "mais_populares":
                    List<Recipe> popular = GetMostPopularRecipes(int.MaxValue);
                    recipes = recipes
                         .OrderBy(r => popular.FindIndex(p => p.Id == r.Id))
                        .ToList();
                    break;

                case "newest":
                case "recent":
                case "recentes":
                case "mais_recentes":
                default:
                    List<Recipe> newest = GetNewestRecipes(int.MaxValue);
                    recipes = recipes
                         .OrderBy(r => newest.FindIndex(n => n.Id == r.Id))
                        .ToList();
                    break;
            }

            return recipes
                .Skip(skip)
                .Take(take)
                .ToList();
        }

        public List<Recipe> GetTopRatedRecipes(int countTopRatedRecipe)
        {
           return _recipeRepo.RetrieveAll()
          .Where(r => r.IsApproved)
          .OrderByDescending(r => (decimal)(_ratingService.GetAverageRating(r.Id) ?? 0m)) 
          .ThenByDescending(r => _ratingService.GetRatingCount(r.Id))                   
          .ThenByDescending(r => r.Id)                                                   
          .Take(countTopRatedRecipe)
          .ToList();
        }

        public List<Recipe> GetMostPopularRecipes(int countMostPopularRecipe)
        {

            List<Recipe> approvedRecipes = _recipeRepo.RetrieveAll()
                    .Where(r => r.IsApproved)
                    .ToList();

            Dictionary<int, int> ratingCounts = _ratingService.GetAllRatingCounts();
            Dictionary<int, int> favoriteCounts = _userFavRepo.RetrieveAll()
             .GroupBy(f => f.RecipeId)
             .ToDictionary(g => g.Key, g => g.Count());
            Dictionary<int, int> commentCounts = _commentService.GetAllCommentCounts();
            Dictionary<int, decimal> averageRatings = _ratingService.GetAllAverageRatings();

            return approvedRecipes
                .OrderByDescending(r =>
                {
                    // 1. Obter as contagens e a média calculada
                    int rCount = ratingCounts.GetValueOrDefault(r.Id, 0);
                    int fCount = favoriteCounts.GetValueOrDefault(r.Id, 0);
                    int cCount = commentCounts.GetValueOrDefault(r.Id, 0);
                    decimal avg = averageRatings.GetValueOrDefault(r.Id, 0.0m);

                    return CalculatePopularityScore(avg, rCount, fCount, cCount);
  
                })
                .ThenByDescending(r => r.Id)
                .Take(countMostPopularRecipe)
                .ToList();

        }

        public List<Recipe> GetNewestRecipes(int countNewestRecipes)
        {
            return _recipeRepo.RetrieveAll()
                .Where(r => r.IsApproved)
                .OrderByDescending(r => r.PublishedRecipeDate ?? DateTime.MinValue)
                .ThenByDescending(r => r.Id)
                .Take(countNewestRecipes)
                .ToList();
        }

        public List<Recipe> GetRelatedRecipes(int currentRecipeId, int categoryId, int authorId, int count = 4)
        {
            
            List<Recipe> relatedRecipes = _recipeRepo.RetrieveAll()
                .Where(r => r.IsApproved && r.Id != currentRecipeId && r.CategoryId == categoryId)
                .OrderByDescending(r => r.Id)
                .Take(count)
                .ToList();

           
            if (relatedRecipes.Count < count)
            {
                int missingCount = count - relatedRecipes.Count;

                List<int> existingIds = relatedRecipes.Select(r => r.Id).ToList();
                existingIds.Add(currentRecipeId);

                List<Recipe> authorRecipes = _recipeRepo.RetrieveAll()
                    .Where(r => r.IsApproved && !existingIds.Contains(r.Id) && r.UserId == authorId)
                    .OrderByDescending(r => r.Id)
                    .Take(missingCount)
                    .ToList();

                relatedRecipes.AddRange(authorRecipes);
            }

            return relatedRecipes;
        }

        public void UpdateRecipe(Recipe updatedRecipeInput, int currentUserId, Stream? newImageStream, string? newOriginalFileName)
        {
            
            Recipe? existingRecipe = _recipeRepo.Retrieve(updatedRecipeInput.Id);
            if (existingRecipe == null)
                throw new Exception("Operação Falhada : Receita não encontrada.");

            
            Users? currentUser = _userRepo.Retrieve(currentUserId);
            if (currentUser == null)
                throw new Exception("Operação Falhada : Utilizador inválido.");

            bool isAuthor = existingRecipe.UserId == currentUserId;
            bool isAdmin = currentUser.IsAdmin;

            if (!isAuthor && !isAdmin)
                throw new Exception("Operação Negada: Não tem permissão para editar esta receita.");

           
            if (_catRepo.Retrieve(updatedRecipeInput.CategoryId) == null)
                throw new Exception("Operação Falhada :: A Categoria selecionada não existe.");

           
            if (_diffRepo.Retrieve(updatedRecipeInput.DifficultyId) == null)
                throw new Exception("Operação Falhada : O Nível de Dificuldade selecionado não existe.");


            string finalImageUrl = existingRecipe.ImageUrl; 

           
            if (newImageStream != null && !string.IsNullOrEmpty(newOriginalFileName))
            {
                
                string savedFileName = _fileService.SaveImage(newImageStream, newOriginalFileName, "images/recipes");

               
                finalImageUrl = existingRecipe.RecipeImage(savedFileName);

                
                _fileService.DeleteImage(existingRecipe.ImageUrl, "images/recipes");
            }

            else if (!string.IsNullOrWhiteSpace(updatedRecipeInput.ImageUrl) && updatedRecipeInput.ImageUrl != existingRecipe.ImageUrl)
            {
                
                finalImageUrl = existingRecipe.RecipeImage(updatedRecipeInput.ImageUrl.Trim());

                
                _fileService.DeleteImage(existingRecipe.ImageUrl, "images/recipes");
            }

           
            bool recipeIsApproved = isAdmin ? existingRecipe.IsApproved : false;


           
        
            DateTime? updatedDate = isAdmin ? existingRecipe.PublishedRecipeDate : null;

           
            Recipe pendingRecipe = new Recipe(
                existingRecipe.Id,                  
                updatedRecipeInput.CategoryId,
                updatedRecipeInput.DifficultyId,
                existingRecipe.UserId,             
                updatedRecipeInput.Title,
                updatedRecipeInput.PreparationMethod,
                updatedRecipeInput.PreparationTime,
                existingRecipe.Rating,              
                finalImageUrl,
                recipeIsApproved,
                updatedDate
               
            );

            
            _recipeRepo.Update(pendingRecipe);
        }

        private void DeleteRecipeDependencies(int recipeId)
        {
          
            List<RecipeIngredient> recipeIngredients = _riRepo.RetrieveAll()
                .Where(ri => ri.RecipeId == recipeId)
                .ToList();

            foreach (RecipeIngredient ri in recipeIngredients)
            {
                _riRepo.Delete(ri.RecipeIngredientId);
            }

            _commentService.RemoveAllCommentsByRecipeId(recipeId);

        
            List<Rating> ratings = _ratingRepo.RetrieveAll()
                .Where(r => r.RecipeId == recipeId)
                .ToList();

            foreach (Rating rating in ratings)
            {
                _ratingRepo.Delete(rating.RatingId);
            }


            List<UserFavourites> favourites = _userFavRepo.RetrieveAll()
                .Where(f => f.RecipeId == recipeId)
                .ToList();

            foreach (UserFavourites fav in favourites)
            {
                _userFavRepo.Delete(fav.Id); 
            }

            Recipe? recipe = _recipeRepo.Retrieve(recipeId);
            if (recipe != null && !string.IsNullOrEmpty(recipe.ImageUrl))
            {
                _fileService.DeleteImage(recipe.ImageUrl, "images/recipes");
            }

        }

        public void DeleteRecipesByUserId(int userId)
        {
            List<Recipe> userRecipes = _recipeRepo.RetrieveAll()
                .Where(r => r.UserId == userId)
                .ToList();

            foreach (Recipe recipe in userRecipes)
            {
                
                DeleteRecipeDependencies(recipe.Id);

              
                _recipeRepo.Delete(recipe.Id);
            }
        }


        public void AdminDeletePendingRecipes(int administratorId, int recipeId)
        {

            Users? administrator = _userRepo.Retrieve(administratorId);
            if (administrator == null || !administrator.IsAdmin == true)
                throw new Exception("Operação Negada: Apenas administradores podem eliminar receitas pendentes.");

            DeleteRecipeDependencies(recipeId);

            _recipeRepo.Delete(recipeId);
        }

        public void DeletePublishedRecipe(int currentUserId, int recipeId)
        {
            Recipe? recipe = _recipeRepo.Retrieve(recipeId);
            if (recipe == null)
                throw new Exception("Operação Falhada : Receita não existe.");

           
            Users? currentUser = _userRepo.Retrieve(currentUserId);
            if (currentUser == null)
                throw new Exception("Operação Falhada : Utilizador não existe.");

            
            bool isAuthor = recipe.UserId == currentUserId;
            bool isAdmin = currentUser.IsAdmin;

            if (!isAuthor && !isAdmin)
            {
                throw new Exception("Operação Negada: Não tem permissão para eliminar esta receita.");
            }

           
            DeleteRecipeDependencies(recipeId);

           
            _recipeRepo.Delete(recipeId);
        }


        public void ApproveRecipe(int recipeId, int adminId)
        {

            Users? admin = _userRepo.Retrieve(adminId);
            if (admin == null || !admin.IsAdmin)
                throw new Exception("Operação Falhada : Apenas Administradores podem aprovar receitas.");


            List<RecipeIngredient> ingredientes = _riService.GetRecipeIngredientsByRecipe(recipeId);
            if (ingredientes.Count == 0)
                throw new Exception("Operação Falhada : Receita não pode ser aprovada sem ingredientes.");


            Recipe? recipe = _recipeRepo.Retrieve(recipeId);
            if (recipe == null) throw new Exception("Operação Falhada : Receita não existe.");


            
            Recipe approvedRecipe = new Recipe(
                recipe.Id,
                recipe.CategoryId,
                recipe.DifficultyId,
                recipe.UserId,
                recipe.Title,
                recipe.PreparationMethod,
                recipe.PreparationTime,
                recipe.Rating,
                recipe.ImageUrl,
                true,             
                DateTime.Now      
            );


            _recipeRepo.Update(approvedRecipe);
        }

        public decimal CalculatePopularityScore(decimal avg, int ratingCount, int favoriteCount, int commentCount)
        {
            
            decimal protectedAvg = avg == 0 ? 1 : avg;
            int protectedRatings = ratingCount == 0 ? 1 : ratingCount;
            int protectedFavorites = favoriteCount == 0 ? 1 : favoriteCount;
            int protectedComments = commentCount == 0 ? 1 : commentCount;

            
            int activeCriteriaCount = 0;
            if (ratingCount > 0) activeCriteriaCount++;
            if (favoriteCount > 0) activeCriteriaCount++;
            if (commentCount > 0) activeCriteriaCount++;

            int multiplier = activeCriteriaCount == 0 ? 1 : activeCriteriaCount;

            decimal baseScore = protectedAvg * protectedRatings * protectedFavorites * protectedComments;
            return baseScore * multiplier;
        }

        private string RemoveAccents(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            string normalized = text.Normalize(System.Text.NormalizationForm.FormD);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (char c in normalized)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower();
        }



    }
}
