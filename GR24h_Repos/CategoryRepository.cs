using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using Microsoft.Data.SqlClient;

namespace GR24h_Repos
{
    public class CategoryRepository : IRepository <Category>
    {
        private readonly string _tableName = "[Category]";

        private Category Parse(SqlDataReader dataReader)
        {
            int id = Convert.ToInt32(dataReader["Category_id"]);
            string Name = Convert.ToString(dataReader["Category_Name"]) ?? string.Empty;

            return new Category(id, Name);
        }

        public Category Create(Category entityCategory)
        {
            string sql = $"INSERT INTO {_tableName} (Category_Name) VALUES (@name)";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@name", entityCategory.Name);

            int newId = Sql.ExecuteNonQuery(cmd);
            return Retrieve(newId)!;
        }

        public Category? Retrieve(int id)
        {
            using (SqlConnection conn = Sql.GetConnection())
            {
                string sql = $"SELECT * FROM {_tableName} WHERE Category_id = @id";
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

        public List<Category> RetrieveAll()
        {
            List<Category> list = new List<Category>();

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

        public void Update(Category entityCategory)
        {
            string sql = $@"UPDATE {_tableName} SET 
                            Category_Name = @name 
                            WHERE Category_id = @id";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@name", entityCategory.Name);
            cmd.Parameters.AddWithValue("@id", entityCategory.Id);

            Sql.ExecuteNonQuery(cmd);
        }

        public void Delete(int id)
        {
            string sql = $"DELETE FROM {_tableName} WHERE Category_id = @id";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@id", id);
            Sql.ExecuteNonQuery(cmd);
        }
    }
}
