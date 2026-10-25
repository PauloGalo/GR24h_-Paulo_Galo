using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR24h_Models
{
    public class CommentDTO
    {
        public Comment Comment { get; set; }
        public string Username { get; set; }

        public CommentDTO(Comment comment, string username)
        {
            Comment = comment;
            Username = username;
        }
    }


    public class Comment
    {
        public int CommentId { get; private set; }
        public int UserId { get; private set; }
        public int RecipeId { get; private set; }


        public int? ParentCommentId { get; private set; }
        public string? CommentContent { get; private set; } = null!;
        public DateTime CommentDate { get; private set; }

        public Comment(
            int commentId,
            int userId,
            int recipeId,
            int? parentCommentId,
            string? valueCommentContent,
            DateTime valueCommentDate)
        {

            this.ParentCommentId = parentCommentId;
            this.CommentId = commentId;
            this.UserId = userId;
            this.RecipeId = recipeId;
            this.CommentContent = CommentCommentContent(valueCommentContent);
            this.CommentDate = CommentCommentDate(valueCommentDate);
        }

        public string? CommentCommentContent(string? valueCommentContent)
        {
            if (string.IsNullOrWhiteSpace(valueCommentContent))
                return null;

            valueCommentContent = valueCommentContent.Trim();

            string[] words = valueCommentContent.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);


            if (words.Length > 5 && valueCommentContent == valueCommentContent.ToUpper())
            {
                valueCommentContent = valueCommentContent.ToLower();
                valueCommentContent = char.ToUpper(valueCommentContent[0]) + valueCommentContent.Substring(1);
            }


            if (valueCommentContent.Length < 2 || valueCommentContent.Length > 300)
                throw new ArgumentException("Comentário deve conter entre 2 a 300 caracteres.Tente novamente.");

            return valueCommentContent;
        }

        public DateTime CommentCommentDate(DateTime valueCommentDate)
        {
            if (valueCommentDate == default || valueCommentDate == DateTime.MinValue)
                throw new ArgumentException("Data inválida.");

            return valueCommentDate;      
        }
    }
}
