using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using Microsoft.Data.SqlClient;

namespace GR24h_Repos
{
    public class UsersRepository : IRepository <Users>
    {
        private readonly string _tableName = "[Users]";

        private Users Parse(SqlDataReader dataReader)
        {
            int id = Convert.ToInt32(dataReader["Users_id"]);
            string firstName = Convert.ToString(dataReader["FirstName"]) ?? string.Empty;
            string lastName = Convert.ToString(dataReader["LastName"]) ?? string.Empty;
            string username = Convert.ToString(dataReader["Username"]) ?? string.Empty;
            string email = Convert.ToString(dataReader["Email"]) ?? string.Empty;
            string password = Convert.ToString(dataReader["Password"]) ?? string.Empty;
            bool isAdmin = Convert.ToBoolean(dataReader["IsAdmin"]);
            bool isLocked = Convert.ToBoolean(dataReader["IsLocked"]);
            int failedLoginAttempts = Convert.ToInt32(dataReader["FailedLoginAttempts"]);

            return new Users(id, firstName, lastName, username, email, password, isAdmin, isLocked, failedLoginAttempts);
        }

        public Users Create(Users entityUser)
        {
            string sql = $@"INSERT INTO {_tableName} 
                            (FirstName, LastName,Username, Email, Password, IsAdmin, IsLocked,FailedLoginAttempts) 
                            VALUES (@fName, @lName, @user, @email, @pass, @admin, @locked, @flattempts)";
                            
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@fName", entityUser.FirstName);
            cmd.Parameters.AddWithValue("@lName", entityUser.LastName);
            cmd.Parameters.AddWithValue("@user", entityUser.Username);
            cmd.Parameters.AddWithValue("@email", entityUser.Email);
            cmd.Parameters.AddWithValue("@pass", entityUser.Password);
            cmd.Parameters.AddWithValue("@admin", entityUser.IsAdmin);
            cmd.Parameters.AddWithValue("@locked", entityUser.IsLocked);
            cmd.Parameters.AddWithValue("@flattempts", entityUser.FailedLoginAttempts);


            int newId = Sql.ExecuteNonQuery(cmd);
            return Retrieve(newId)!;
        }

        public Users? Retrieve(int id)
        {
           
            using (SqlConnection conn = Sql.GetConnection())
            {
                string sql = $"SELECT * FROM {_tableName} WHERE Users_id = @id";
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

        public List<Users> RetrieveAll()
        {
            List<Users> list = new List<Users>();
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

        public void Update(Users entityUser)
        {
            string sql = $@"UPDATE {_tableName} SET 
                            FirstName = @fName, 
                            LastName = @lName,                         
                            Username = @user, 
                            Email = @email, 
                            Password = @pass, 
                            IsAdmin = @admin, 
                            IsLocked = @locked,
                            FailedLoginAttempts = @flAttempts
                            WHERE Users_id = @id";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@fName", entityUser.FirstName);
            cmd.Parameters.AddWithValue("@lName", entityUser.LastName);
            cmd.Parameters.AddWithValue("@user", entityUser.Username);
            cmd.Parameters.AddWithValue("@email", entityUser.Email);
            cmd.Parameters.AddWithValue("@pass", entityUser.Password);
            cmd.Parameters.AddWithValue("@admin", entityUser.IsAdmin);
            cmd.Parameters.AddWithValue("@locked", entityUser.IsLocked);
            cmd.Parameters.AddWithValue("@flAttempts", entityUser.FailedLoginAttempts);
            cmd.Parameters.AddWithValue("@id", entityUser.Id);

            Sql.ExecuteNonQuery(cmd);
        }

        public void Delete(int id)
        {
            string sql = $"DELETE FROM {_tableName} WHERE Users_id = @id";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@id", id);
            Sql.ExecuteNonQuery(cmd);
        }
    }
}
