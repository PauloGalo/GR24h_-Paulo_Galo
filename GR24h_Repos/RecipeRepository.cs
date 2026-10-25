using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using Microsoft.Data.SqlClient;

namespace GR24h_Repos
{
    public class RecipeRepository : IRepository <Recipe>
    {

        private readonly string _tableName = "[Recipe]";

        private Recipe Parse(SqlDataReader dataReader)
        {
            int id = Convert.ToInt32(dataReader["Recipe_id"]);
            int categoryId = Convert.ToInt32(dataReader["Category_id"]);          
            int difficultyId = Convert.ToInt32(dataReader["Difficulty_id"]);         
            int userId = Convert.ToInt32(dataReader["Users_id"]);
            string title = Convert.ToString(dataReader["Recipe_Title"]) ?? string.Empty;
            string preparationMethod = Convert.ToString(dataReader["Recipe_PreparationMethod"]) ?? string.Empty;
            string preparationTime = Convert.ToString(dataReader["Recipe_PreparationTime"]) ?? string.Empty;
            decimal? rating = dataReader["Recipe_Rating"] == DBNull.Value
                  ? null
                  : Convert.ToDecimal(dataReader["Recipe_Rating"]);
            string imageUrl = Convert.ToString(dataReader["Recipe_ImageUrl"]) ?? string.Empty;
            bool isApproved = Convert.ToBoolean(dataReader["Recipe_IsApproved"]);
            DateTime? recipeDate = dataReader["Recipe_Date"] == DBNull.Value
                ? null
                : Convert.ToDateTime(dataReader["Recipe_Date"]);


            return new Recipe(
                id,
                categoryId,
                difficultyId,
                userId,
                title,
                preparationMethod,
                preparationTime,
                rating,
                imageUrl,
                isApproved,
                recipeDate

            );
        }

        public Recipe Create(Recipe entityRecipe)
        {
            string sql = $@"INSERT INTO {_tableName} 
                            (Category_id, Difficulty_id, Users_id, Recipe_Title, Recipe_PreparationMethod, Recipe_PreparationTime, Recipe_Rating, Recipe_ImageUrl, Recipe_IsApproved, Recipe_Date) 
                            VALUES (@catId, @diffId, @userId, @title, @method, @time, @rating, @imgUrl, @approved, @rDate)";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@catId", entityRecipe.CategoryId);
            cmd.Parameters.AddWithValue("@diffId", entityRecipe.DifficultyId);
            cmd.Parameters.AddWithValue("@userId", entityRecipe.UserId);
            cmd.Parameters.AddWithValue("@title", entityRecipe.Title);
            cmd.Parameters.AddWithValue("@method", entityRecipe.PreparationMethod);
            cmd.Parameters.AddWithValue("@time", entityRecipe.PreparationTime);
            cmd.Parameters.AddWithValue("@rating", (entityRecipe.Rating.HasValue ? entityRecipe.Rating.Value : DBNull.Value));
            cmd.Parameters.AddWithValue("@imgUrl", entityRecipe.ImageUrl);
            cmd.Parameters.AddWithValue("@approved", entityRecipe.IsApproved);
            cmd.Parameters.AddWithValue("@rDate", (entityRecipe.PublishedRecipeDate.HasValue ? entityRecipe.PublishedRecipeDate.Value : DBNull.Value));

            int newId = Sql.ExecuteNonQuery(cmd);
            return Retrieve(newId)!;
        }

        public Recipe? Retrieve(int id)
        {
            using (SqlConnection conn = Sql.GetConnection())
            {
               string sql = $@" SELECT Recipe.Recipe_id,Recipe.Category_id,Recipe.Difficulty_id, Recipe.Users_Id, 
                            Recipe.Recipe_Title, Recipe.Recipe_PreparationMethod, Recipe.Recipe_PreparationTime, 
                            Recipe.Recipe_Rating, Recipe.Recipe_ImageUrl, Recipe.Recipe_IsApproved, Recipe.Recipe_Date,
                            Category.Category_Name, Difficulty.Difficulty_Name, Users.Username
                            FROM {_tableName} Recipe
                            INNER JOIN [Category] Category ON Recipe.category_id = Category.Category_id
                            INNER JOIN [Difficulty] Difficulty ON Recipe.Difficulty_id = Difficulty.Difficulty_id
                            INNER JOIN [Users] Users ON Recipe.Users_id = Users.Users_id
                            WHERE Recipe.Recipe_id = @id";


                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@id", id);

                using (SqlDataReader reader = Sql.ExecuteQuery(cmd, conn))
                {
                    if (reader.Read())
                    {
                        return Parse(reader);
                    }
                    return null;
                }
            }

        }

        public List<Recipe> RetrieveAll()
        {
            List<Recipe> list = new List<Recipe>();

            using (SqlConnection conn = Sql.GetConnection())
            {
                string sql = $@" SELECT Recipe.Recipe_id,Recipe.Category_id,Recipe.Difficulty_id, Recipe.Users_Id, 
                             Recipe.Recipe_Title, Recipe.Recipe_PreparationMethod, Recipe.Recipe_PreparationTime, 
                             Recipe.Recipe_Rating, Recipe.Recipe_ImageUrl, Recipe.Recipe_IsApproved, Recipe.Recipe_Date,
                             Category.Category_Name, Difficulty.Difficulty_Name, Users.Username
                             FROM {_tableName} Recipe
                             INNER JOIN [Category] Category ON Recipe.category_id = Category.Category_id
                             INNER JOIN [Difficulty] Difficulty ON Recipe.Difficulty_id = Difficulty.Difficulty_id
                             INNER JOIN [Users] Users ON Recipe.Users_id = Users.Users_id";

                SqlCommand cmd = new SqlCommand(sql);

                using (SqlDataReader reader = Sql.ExecuteQuery(cmd, conn))
                {
                    while (reader.Read())
                    {
                        list.Add(Parse(reader));
                    }
                    return list;
                }
            }

        }


        public void Update(Recipe entityRecipe)
        {
            string sql = $@"UPDATE {_tableName} SET 
                            Category_id = @catId, 
                            Difficulty_id = @diffId, 
                            Users_id = @userId, 
                            Recipe_Title = @title, 
                            Recipe_PreparationMethod = @method, 
                            Recipe_PreparationTime = @time, 
                            Recipe_Rating = @rating, 
                            Recipe_ImageUrl = @imgUrl,
                            Recipe_IsApproved = @approved,
                            Recipe_Date = @rDate
                            WHERE Recipe_id = @id";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@catId", entityRecipe.CategoryId);
            cmd.Parameters.AddWithValue("@diffId", entityRecipe.DifficultyId);
            cmd.Parameters.AddWithValue("@userId", entityRecipe.UserId);
            cmd.Parameters.AddWithValue("@title", entityRecipe.Title);
            cmd.Parameters.AddWithValue("@method", entityRecipe.PreparationMethod);
            cmd.Parameters.AddWithValue("@time", entityRecipe.PreparationTime);
            cmd.Parameters.AddWithValue("@rating", (entityRecipe.Rating.HasValue ? entityRecipe.Rating.Value : DBNull.Value));
            cmd.Parameters.AddWithValue("@imgUrl", entityRecipe.ImageUrl);
            cmd.Parameters.AddWithValue("@approved", entityRecipe.IsApproved);
            cmd.Parameters.AddWithValue("@rDate", (entityRecipe.PublishedRecipeDate.HasValue ? entityRecipe.PublishedRecipeDate.Value : DBNull.Value));
            cmd.Parameters.AddWithValue("@id", entityRecipe.Id);

            Sql.ExecuteNonQuery(cmd);
        }

        public void Delete(int id)
        {
            string sql = $"DELETE FROM {_tableName} WHERE Recipe_id = @id";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@id", id);

            Sql.ExecuteNonQuery(cmd);
        }


    }
}
