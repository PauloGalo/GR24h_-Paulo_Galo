using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using Microsoft.Data.SqlClient;

namespace GR24h_Repos
{
    public class CommentRepository : IRepository <Comment>
    {

        private readonly string _tableName = "[Comment]";

        private Comment Parse(SqlDataReader dataReader)
        {
            int commentId = Convert.ToInt32(dataReader["Comment_id"]);
            int userId = Convert.ToInt32(dataReader["Users_id"]);
            int recipeId = Convert.ToInt32(dataReader["Recipe_id"]);

            int? parentCommentId = dataReader["ParentComment_id"] == DBNull.Value
                                    ? null
                                    : (int?)Convert.ToInt32(dataReader["ParentComment_id"]);

            string? commentContent = dataReader["Comment_content"] == DBNull.Value
                         ? null
                         : Convert.ToString(dataReader["Comment_content"]);
            DateTime commentDate = Convert.ToDateTime(dataReader["Comment_date"]);


            return new Comment(commentId, userId, recipeId, parentCommentId, commentContent, commentDate);

        }

        public Comment Create(Comment entityComment)
        {

            string sql = $@"INSERT INTO {_tableName} (Users_id, Recipe_id, ParentComment_id, Comment_content, Comment_date) 
                            VALUES (@uId, @rId, @parentId, @commentContent, @commentDate)";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@uId", entityComment.UserId);
            cmd.Parameters.AddWithValue("@rId", entityComment.RecipeId);
            cmd.Parameters.AddWithValue("@parentId", (entityComment.ParentCommentId.HasValue ? entityComment.ParentCommentId.Value : DBNull.Value));
            cmd.Parameters.AddWithValue("@commentContent", (string.IsNullOrEmpty(entityComment.CommentContent) ? DBNull.Value : entityComment.CommentContent));
            cmd.Parameters.AddWithValue("@commentDate", entityComment.CommentDate);

            int newId = Sql.ExecuteNonQuery(cmd);
            return Retrieve(newId)!;
        }

        public Comment? Retrieve(int id)
        {
            using (SqlConnection conn = Sql.GetConnection())
            {
                string sql = $@" SELECT * FROM {_tableName} WHERE Comment.Comment_id = @id";

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

        public List<Comment> RetrieveAll()
        {
            List<Comment> list = new List<Comment>();

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

        public void Update(Comment entityComment)
        {

            string sql = $@"UPDATE {_tableName} SET 
                            Users_id = @uId,
                            Recipe_id = @rId,
                            ParentComment_id = @parentId,
                            Comment_content = @commentContent, 
                            Comment_date = @commentDate 
                            WHERE Comment_id = @id";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@uId", entityComment.UserId);
            cmd.Parameters.AddWithValue("@rId", entityComment.RecipeId);
            cmd.Parameters.AddWithValue("@parentId", (entityComment.ParentCommentId.HasValue ? entityComment.ParentCommentId.Value : DBNull.Value));
            cmd.Parameters.AddWithValue("@commentContent", (string.IsNullOrEmpty(entityComment.CommentContent) ? DBNull.Value : entityComment.CommentContent));
            cmd.Parameters.AddWithValue("@commentDate", entityComment.CommentDate);
            cmd.Parameters.AddWithValue("@id", entityComment.CommentId);

            Sql.ExecuteNonQuery(cmd);
        }

        public void Delete(int id)
        {
            string sql = $"DELETE FROM {_tableName} WHERE Comment_id = @id";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@id", id);
            Sql.ExecuteNonQuery(cmd);
        }
    }
}
