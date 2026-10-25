using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR24h_Models
{
    public class Rating
    {
        public int RatingId { get; private set; }
        public int UserId { get; private set; }
        public int RecipeId { get; private set; }

     

        public int? RatingScore { get; private set; }
        public DateTime RatingDate { get; private set; }


        public Rating(
            int ratingId,
            int userId,
            int recipeId,
            int? valueRatingScore,
            DateTime valueRatingDate)
        {

            this.RatingId = ratingId;
            this.UserId = userId;
            this.RecipeId = recipeId;
            this.RatingScore = RecipeRatingScore(valueRatingScore);
            this.RatingDate = RecipeRatingDate(valueRatingDate);
        }

        public int? RecipeRatingScore(int? valueRatingScore)
        {

            if (valueRatingScore == null)
            {
                return null;
            }


            if (valueRatingScore < 1 || valueRatingScore > 5)
                throw new ArgumentException("Rating deve ser avaliado entre 1 e 5 estrelas. Tente novamente.");

            return valueRatingScore;
        }

        public DateTime RecipeRatingDate(DateTime valueRatingDate)
        {
            if (valueRatingDate == default || valueRatingDate == DateTime.MinValue)
                throw new ArgumentException("Data inválida.");

            return valueRatingDate;
        }

    }
}
