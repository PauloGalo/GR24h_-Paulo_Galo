using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace GR24h_Models
{
    public class RecipeDetailsDTO
    {
        public Recipe Recipe { get; set; }
        public string CategoryName { get; set; }
        public string DifficultyName { get; set; }
        public string AuthorUsername { get; set; }

        public decimal PopularityScore { get; set; }

        public RecipeDetailsDTO(
            Recipe recipe,
            string categoryName,
            string difficultyName,
            string authorUsername,
            decimal popularityScore)
        {
            Recipe = recipe;
            CategoryName = categoryName;
            DifficultyName = difficultyName;
            AuthorUsername = authorUsername;
            PopularityScore = popularityScore;
        }
    }
    public class Recipe
    {
        public int Id { get; private set; }
        public int CategoryId { get; private set; }
        public int DifficultyId { get; private set; }
        public int UserId { get; private set; }

       

        public string Title { get; private set; } = null!;
        public string PreparationMethod { get; private set; } = null!;
        public string PreparationTime { get; private set; } = null!;
        public decimal? Rating { get; private set; }


        public string ImageUrl { get; private set; } = null!;
        public bool IsApproved { get; private set; }

        public DateTime? PublishedRecipeDate { get; private set;}
        public Recipe(
            int id,
            int categoryId,
            int difficultyId,
            int userId,
            string valueTitle,
            string valuePreparationMethod,
            string valuePreparationTime,
            decimal? valueRecipeRating,
            string valueImageUrl,
            bool isApproved,
            DateTime ? publishedRecipedate = null)
        {

            this.Id = id;
            this.CategoryId = categoryId;
            this.DifficultyId = difficultyId;
            this.UserId = userId;


            this.Title = RecipeTitle(valueTitle);
            this.PreparationMethod = RecipePreparationMethod(valuePreparationMethod);
            this.PreparationTime = RecipePreparationTime(valuePreparationTime);

            this.Rating = RecipeRating(valueRecipeRating);
            this.ImageUrl = RecipeImage(valueImageUrl);
            this.IsApproved = isApproved;
            this.PublishedRecipeDate = publishedRecipedate;


        }


        public string RecipeTitle(string valueTitle)
        {
            if (string.IsNullOrWhiteSpace(valueTitle))
                throw new ArgumentException("Campo obrigatório");

            valueTitle = valueTitle.Trim();

            if (valueTitle.Length < 5 || valueTitle.Length > 50)
                throw new ArgumentException("Título de Receita deve conter entre 5 a 50 caracteres.Tente novamente.");

            if (!valueTitle.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                throw new ArgumentException("Título de Receita deve conter apenas letras e espaços.Tente novamente");

            return valueTitle;
        }

        public string RecipePreparationMethod(string valuePreparationMethod)
        {
            if (string.IsNullOrWhiteSpace(valuePreparationMethod))
                throw new ArgumentException("Campo obrigatório");

            valuePreparationMethod = valuePreparationMethod.Trim();

            if (valuePreparationMethod.Length < 50 || valuePreparationMethod.Length > 3000)
                throw new ArgumentException("Método de Preparação de Receita inserida deve conter entre 50 a 3000 caracteres.Tente novamente.");



            return valuePreparationMethod;
        }

        public string RecipePreparationTime(string valuePreparationTime)
        {
            if (string.IsNullOrWhiteSpace(valuePreparationTime))
                throw new ArgumentException("Campo obrigatório");

            valuePreparationTime = valuePreparationTime.Trim();

            if (valuePreparationTime.Length < 4 || valuePreparationTime.Length > 10)
                throw new ArgumentException("Tempo de Preparação de Receita inserida deve conter entre 4 a 10 caracteres.Tente novamente.");


            if (!valuePreparationTime.EndsWith("min"))
                throw new ArgumentException("Tempo de Preparação de Receita somente aceita unidade de tempo em minutos e com diminutivo min.Tente novamente.");


            if (!valuePreparationTime.All(c2 => char.IsLetterOrDigit(c2) || char.IsWhiteSpace(c2)))
                throw new ArgumentException("Tempo de Preparação de Receita deve conter apenas números,letras e espaços.Tente novamente.");

            return valuePreparationTime;
        }


        public decimal? RecipeRating(decimal? valueRecipeRating)
        {
            
            if (valueRecipeRating == null)
            {
                return null;

            }


            if (valueRecipeRating < 1 || valueRecipeRating > 5)
                throw new ArgumentException("Rating deve ser avaliado entre 1 e 5 estrelas. Tente novamente.");

            return valueRecipeRating;
        }


        public void UpdateAverageRating(decimal? newRating)
        {
            this.Rating = RecipeRating(newRating);
        }

        public void UpdateImage(string newImageUrl)
        {
            this.ImageUrl = RecipeImage(newImageUrl);
        }

        public string RecipeImage(string? valueImageUrl)
        {
            if (string.IsNullOrWhiteSpace(valueImageUrl))

                return "default-recipe.jpg";

            valueImageUrl = valueImageUrl.Trim();

           
            if (Uri.TryCreate(valueImageUrl, UriKind.Absolute, out Uri? uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttps))
            {
              
                string pathWithoutQuery = uriResult.AbsolutePath;
                string urlExtension = System.IO.Path.GetExtension(pathWithoutQuery).ToLowerInvariant();

                string[] allowedUrlExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

                if (!string.IsNullOrEmpty(urlExtension) && !allowedUrlExtensions.Contains(urlExtension))
                {
                    throw new ArgumentException("Apenas são permitidas imagens em ficheiros (.jpg, .jpeg, .png e .webp.). Tente novamente.");
                }

                return valueImageUrl;
            }

            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
            string extension = System.IO.Path.GetExtension(valueImageUrl).ToLowerInvariant();

            if (!string.IsNullOrEmpty(extension) && !allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Apenas são permitidas imagens com ficheiros (.jpg, .jpeg, .png e .webp.). Tente novamente.");
            }

            return valueImageUrl;
        }
        public string GetFormattedImageUrl()
        {
            if (string.IsNullOrEmpty(this.ImageUrl))
                return "/images/default-recipe.jpg";

            if (this.ImageUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                return this.ImageUrl;

            return $"/images/recipes/{this.ImageUrl}";
        }

        public void RecipeIsApproved()
        {
            this.IsApproved = true;
        }

        

    }
}
