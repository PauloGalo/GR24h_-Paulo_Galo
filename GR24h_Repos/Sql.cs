using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace GR24h_Repos
{
    internal class Sql
    {

        private const string _connectionString = @"Server=localhost\SQLEXPRESS04;Database=GR24horas_Public_DB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;Connect Timeout=60;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public static int ExecuteNonQuery(SqlCommand cmd)
        {
            using (SqlConnection conn = GetConnection())
            {
                cmd.Connection = conn;
                conn.Open();

                if (cmd.CommandText.Trim().ToUpper().StartsWith("INSERT"))
                {
                    cmd.CommandText += "; SELECT CAST(scope_identity() AS int);";
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    return Convert.ToInt32(cmd.ExecuteNonQuery());
                }
            }
        }


        public static SqlDataReader ExecuteQuery(SqlCommand cmd, SqlConnection conn)
        {
            try
            {
                
                if (conn.State != ConnectionState.Open) conn.Open();

                
                cmd.Connection = conn;

                
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception)
            {
                
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close(); 
                }
                conn.Dispose();
                throw; 
            }
        }


    }
}
