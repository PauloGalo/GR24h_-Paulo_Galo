using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using Microsoft.Data.SqlClient;

namespace GR24h_Repos
{
    public class UserFavouritesRepository : IRepository <UserFavourites>
    {
        private readonly string _tableName = "[UserFavourite]";

        private UserFavourites Parse(SqlDataReader dataReader)
        {
            int id = Convert.ToInt32(dataReader["UserFavourite_id"]);
            int userId = Convert.ToInt32(dataReader["Users_id"]);
            int recipeId = Convert.ToInt32(dataReader["Recipe_id"]);

           

            return new UserFavourites(id, userId, recipeId);

        }

        public UserFavourites Create(UserFavourites entityUserFavourites)
        {
            string sql = $@"INSERT INTO {_tableName} (Users_id, Recipe_id) 
                            VALUES (@uId, @rId)";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@uId", entityUserFavourites.UserId);
            cmd.Parameters.AddWithValue("@rId", entityUserFavourites.RecipeId);

            Sql.ExecuteNonQuery(cmd);

            return entityUserFavourites;
        }

        public UserFavourites? Retrieve(int id)
        {
            using (SqlConnection conn = Sql.GetConnection())
            {
              
                string sql = $@" SELECT * FROM {_tableName} WHERE UserFavourite.UserFavourite_id = @id";

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

        public List<UserFavourites> RetrieveAll()
        {
            List<UserFavourites> list = new List<UserFavourites>();

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

        public void Update(UserFavourites entityUserFavourites)
        {
            string sql = $@"UPDATE {_tableName} SET                         
                            Users_id = @uId
                            Recipe_id = @rId
                            WHERE UserFavourite_id = @id";

            SqlCommand cmd = new SqlCommand(sql);

            cmd.Parameters.AddWithValue("@uId", entityUserFavourites.UserId);
            cmd.Parameters.AddWithValue("@rId", entityUserFavourites.RecipeId);
            cmd.Parameters.AddWithValue("@id", entityUserFavourites.Id);

            Sql.ExecuteNonQuery(cmd);
        }

        public void Delete(int id)
        {
            string sql = $"DELETE FROM {_tableName} WHERE UserFavourite_id = @id";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@id", id);

            Sql.ExecuteNonQuery(cmd);
        }


    }
}
