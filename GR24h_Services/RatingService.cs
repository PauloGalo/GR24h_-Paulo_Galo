using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using GR24h_Repos;

namespace GR24h_Services
{
    public class RatingService
    {
        private readonly RatingRepository _ratingRepo;

        public RatingService(RatingRepository ratingRepo)
        {
            _ratingRepo = ratingRepo;
        }

        public void AddRating(Rating rating)
        {
            
            ValidateRatingDate(rating.RatingDate);

           
            bool alreadyRated = _ratingRepo.RetrieveAll()
                .Any(r => r.UserId == rating.UserId && r.RecipeId == rating.RecipeId);

            if (alreadyRated)
            {
                throw new Exception("Operação Falhada : Já avaliaste esta receita. Não é permitido avaliar novamente.");
            }

            _ratingRepo.Create(rating);
        }

        
        private void ValidateRatingDate(DateTime valueRatingDate)
        {
            DateTime now = DateTime.Now;

            if (valueRatingDate < now.AddMinutes(-2) || valueRatingDate > now.AddMinutes(2))
            {
                throw new ArgumentException("Data inválida. A data da avaliação da receita deve ser atual.");
            }
        }

        public Rating GetRatingById(int ratingId)
        {
            if (ratingId <= 0)
                throw new Exception("Operação Falhada : ID de Rating inválido.");

            Rating? rating = _ratingRepo.Retrieve(ratingId);

            if (rating == null)
                throw new Exception("Operação Falhada : Rating solicitado não existe.");

            return rating;           
        }


        public decimal? GetAverageRating(int recipeId)
        {

            List<Rating> ratings = _ratingRepo.RetrieveAll()
           .Where(r => r.RecipeId == recipeId && r.RatingScore.HasValue)
           .ToList();

            if (ratings.Count == 0) return null;

            return (decimal)ratings.Average(r => r.RatingScore!.Value);

        }


        public Rating? GetUserRating(int recipeId, int userId)
        {

            Rating? userRating = _ratingRepo.RetrieveAll()
                .FirstOrDefault(r => r.RecipeId == recipeId && r.UserId == userId);

            return userRating;
        }

        public List<Rating> GetAllRatings()
        {
            return _ratingRepo.RetrieveAll();
        }


        public void RemoveRating(int ratingId)
        {
            if (_ratingRepo.Retrieve(ratingId) == null)
            {
                throw new Exception("Operação Falhada : Avaliação não existe.");
            }

            _ratingRepo.Delete(ratingId);
        }

        public void RemoveAllRatingsByUserId(int userId)
        {
            List<Rating> userRatings = _ratingRepo.RetrieveAll()
                .Where(r => r.UserId == userId)
                .ToList();

            foreach (Rating rating in userRatings)
            {
                _ratingRepo.Delete(rating.RatingId);
            }
        }

        public int GetRatingCount(int recipeId)
        {
            return _ratingRepo.RetrieveAll()
                .Count(r => r.RecipeId == recipeId && r.RatingScore.HasValue);
        }

        
        public Dictionary<int, int> GetAllRatingCounts()
        {
            return _ratingRepo.RetrieveAll()
                .Where(r => r.RatingScore.HasValue)
                .GroupBy(r => r.RecipeId)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public Dictionary<int, decimal> GetAllAverageRatings()
        {
            return _ratingRepo.RetrieveAll()
                .Where(r => r.RatingScore.HasValue)
                .GroupBy(r => r.RecipeId)
                .ToDictionary(
                    g => g.Key,
                    g => (decimal)g.Average(r => r.RatingScore!.Value)
                );
        }

    }
}
