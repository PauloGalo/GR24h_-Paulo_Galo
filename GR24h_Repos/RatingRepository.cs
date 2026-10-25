using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using Microsoft.Data.SqlClient;

namespace GR24h_Repos
{
    public class RatingRepository : IRepository <Rating>
    {
        private readonly string _tableName = "[Rating]";

        private Rating Parse(SqlDataReader dataReader)
        {
            int ratingId = Convert.ToInt32(dataReader["Rating_id"]);
            int userId = Convert.ToInt32(dataReader["Users_id"]);
            int recipeId = Convert.ToInt32(dataReader["Recipe_id"]);
            int? ratingScore = dataReader["Rating_score"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(dataReader["Rating_score"]);
            DateTime ratingDate = Convert.ToDateTime(dataReader["Rating_date"]);

            return new Rating(ratingId, userId, recipeId, ratingScore, ratingDate);
        }

        public Rating Create(Rating entityRating)
        {
            string sql = $@"INSERT INTO {_tableName} (Users_id, Recipe_id, Rating_score, Rating_date) 
                            VALUES (@uId, @rId, @score, @date)";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@uId", entityRating.UserId);
            cmd.Parameters.AddWithValue("@rId", entityRating.RecipeId);
            cmd.Parameters.AddWithValue("@score", (entityRating.RatingScore.HasValue ? entityRating.RatingScore.Value : DBNull.Value));
            cmd.Parameters.AddWithValue("@date", entityRating.RatingDate);

            int newId = Sql.ExecuteNonQuery(cmd);
            return Retrieve(newId)!;
        }

        public Rating? Retrieve(int id)
        {
            using (SqlConnection conn = Sql.GetConnection())
            {
                string sql = $@" SELECT * FROM {_tableName} WHERE Rating.Rating_id = @id";


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

        public List<Rating> RetrieveAll()
        {
            List<Rating> list = new List<Rating>();

            using (SqlConnection conn = Sql.GetConnection())
            {
                string sql = $"SELECT * FROM {_tableName}";

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

        public void Update(Rating entityRating)
        {
            string sql = $@"UPDATE {_tableName} SET                         
                            Users_id = @uId,
                            Recipe_id = @rId,
                            Rating_score = @score, 
                            Rating_date = @date 
                            WHERE Rating_id = @id";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@uId", entityRating.UserId);
            cmd.Parameters.AddWithValue("@rId", entityRating.RecipeId);
            cmd.Parameters.AddWithValue("@score", (entityRating.RatingScore.HasValue ? entityRating.RatingScore.Value: DBNull.Value));
            cmd.Parameters.AddWithValue("@date", entityRating.RatingDate);
            cmd.Parameters.AddWithValue("@id", entityRating.RatingId);

            Sql.ExecuteNonQuery(cmd);
        }

        public void Delete(int id)
        {
            string sql = $"DELETE FROM {_tableName} WHERE Rating_id = @id";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@id", id);
            Sql.ExecuteNonQuery(cmd);
        }
    }
}
