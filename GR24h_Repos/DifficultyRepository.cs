using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using Microsoft.Data.SqlClient;

namespace GR24h_Repos
{
    public class DifficultyRepository : IRepository <Difficulty>
    {
        private readonly string _tableName = "[Difficulty]";

        private Difficulty Parse(SqlDataReader dataReader)
        {
            int id = Convert.ToInt32(dataReader["Difficulty_id"]);
            string Name = Convert.ToString(dataReader["Difficulty_Name"]) ?? string.Empty;

            return new Difficulty(id, Name);
        }

        public Difficulty Create(Difficulty entityDifficulty)
        {
            string sql = $"INSERT INTO {_tableName} (Difficulty_Name) VALUES (@name)";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@name", entityDifficulty.Name);

            int newId = Sql.ExecuteNonQuery(cmd);
            return Retrieve(newId)!;
        }

        public Difficulty? Retrieve(int id)
        {
            using (SqlConnection conn = Sql.GetConnection())
            {
                string sql = $"SELECT * FROM {_tableName} WHERE Difficulty_id = @id";
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

        public List<Difficulty> RetrieveAll()
        {
            List<Difficulty> list = new List<Difficulty>();

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

        public void Update(Difficulty entityDifficulty)
        {
            string sql = $@"UPDATE {_tableName} SET  
                         Difficulty_Name = @name 
                         WHERE Difficulty_id = @id";


            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@name", entityDifficulty.Name);
            cmd.Parameters.AddWithValue("@id", entityDifficulty.Id);

            Sql.ExecuteNonQuery(cmd);
        }

        public void Delete(int id)
        {
            string sql = $"DELETE FROM {_tableName} WHERE Difficulty_id = @id";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@id", id);

            Sql.ExecuteNonQuery(cmd);
        }

    }
}
