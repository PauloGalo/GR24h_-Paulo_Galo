using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using Microsoft.Data.SqlClient;

namespace GR24h_Repos
{
    public class IngredientRepository : IRepository <Ingredient>
    {
        private readonly string _tableName = "[Ingredient]";

        private Ingredient Parse(SqlDataReader dataReader)
        {
            int id = Convert.ToInt32(dataReader["Ingredient_id"]);
            string Name = Convert.ToString(dataReader["Ingredient_Name"]) ?? string.Empty;

            return new Ingredient(id, Name);
        }

        public Ingredient Create(Ingredient entityIngredient)
        {
            string sql = $"INSERT INTO {_tableName} (Ingredient_Name) VALUES (@name)";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@name", entityIngredient.Name);

            int newId = Sql.ExecuteNonQuery(cmd);
            return Retrieve(newId)!;
        }

        public Ingredient? Retrieve(int id)
        {
            using (SqlConnection conn = Sql.GetConnection())
            {
                string sql = $"SELECT * FROM {_tableName} WHERE Ingredient_id = @id";
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

        public List<Ingredient> RetrieveAll()
        {
            List<Ingredient> list = new List<Ingredient>();

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

        public void Update(Ingredient entityIngredient)
        {
            string sql = $@"UPDATE {_tableName} SET 
                         Ingredient_Name = @name 
                         WHERE Ingredient_id = @id";


            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@name", entityIngredient.Name);
            cmd.Parameters.AddWithValue("@id", entityIngredient.Id);
            Sql.ExecuteNonQuery(cmd);
        }

        public void Delete(int id)
        {
            string sql = $"DELETE FROM {_tableName} WHERE Ingredient_id = @id";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@id", id);
            Sql.ExecuteNonQuery(cmd);
        }

    }
}
