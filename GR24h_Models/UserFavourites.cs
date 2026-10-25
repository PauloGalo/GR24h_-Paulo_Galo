using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR24h_Models
{
    public class UserFavourites
    {
        public int Id { get; private set; }

        public int UserId { get; private set; }
        public int RecipeId { get; private set; }

       
        public UserFavourites (int Id, int userId, int recipeId)
        {

            this.Id = Id;
            this.UserId = userId;
            this.RecipeId = recipeId;

        }


        public static UserFavourites CreateFavourite(int userId, int recipeId)
        {
            return new UserFavourites(0, userId, recipeId);
        }


    }
}
