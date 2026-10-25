using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using GR24h_Repos;

namespace GR24h_Services
{
    public class CommentService
    {
        private readonly CommentRepository _commentRepo;
        private readonly RecipeRepository _recipeRepo;
        private readonly UsersRepository _userRepo;

        public CommentService(
            CommentRepository commentRepo,
            RecipeRepository recipeRepo,
            UsersRepository userRepo)
        {
            _commentRepo = commentRepo;
            _recipeRepo = recipeRepo;
            _userRepo = userRepo;
        }

        public void AddComment(Comment newComment)
        {
            ValidateCommentDate(newComment.CommentDate);

            if (_recipeRepo.Retrieve(newComment.RecipeId) == null)
                throw new Exception("Comentário não pode ser adicionado: Receita não existe.");


            if (_userRepo.Retrieve(newComment.UserId) == null)
                throw new Exception("Comentário não pode ser adicionado: Utilizador não existe.");

            _commentRepo.Create(newComment);
        }


        private void ValidateCommentDate(DateTime valueCommentDate)
        {
            DateTime now = DateTime.Now;

            if (valueCommentDate < now.AddMinutes(-2) || valueCommentDate > now.AddMinutes(2))
            {
                throw new ArgumentException("Data inválida. A data do comentário deve ser atual.");
            }
        }
        public Comment GetCommentById(int commentId)
        {
            if (commentId <= 0)
                throw new Exception("Operação Falhada : ID de Comentário inválido.");

            Comment? comment = _commentRepo.Retrieve(commentId);

            if (comment == null)
                throw new Exception("Operação Falhada : Comentário solicitado não existe.");

            return comment;
        }

        public void ReplyToComment(int originalCommentId, Comment reply)
        {

            if (_commentRepo.Retrieve(originalCommentId) == null)
                throw new Exception("Comentário não pode ser respondido : Comentário original já não existe.");

            if (reply.ParentCommentId != originalCommentId)
                throw new Exception("A resposta deve estar vinculada ao ID do comentário original.");

            AddComment(reply);
        }


        public List<CommentDTO> GetCommentsWithUserByRecipe(int recipeId)
        {
            List<Comment> comments = _commentRepo.RetrieveAll()
                .Where(c => c.RecipeId == recipeId)
                .OrderByDescending(c => c.CommentDate)
                .ToList();

            List<CommentDTO> commentsDtoList = new List<CommentDTO>();

            foreach (Comment comment in comments)
            {
                Users? user = _userRepo.Retrieve(comment.UserId);
                string username = user != null ? user.Username : "Utilizador Anónimo";

                commentsDtoList.Add(new CommentDTO(comment, username));
            }

            return commentsDtoList;
        }





        public void DeleteComment(int commentId, int userId, bool isAdmin)
        {
            Comment? comment = _commentRepo.Retrieve(commentId);
            if (comment == null) throw new Exception("Operação falhada:Commentário não encontrado.");


            if (comment.UserId != userId && !isAdmin)
                throw new Exception("Operação falhada:Somente Administradores ou Autor do comentário podem apagar comentários.");

           
            DeleteCommentWithReplies(commentId);
        }

        public void RemoveAllCommentsByRecipeId(int recipeId)
        {
            
            List<Comment> recipeComments = _commentRepo.RetrieveAll()
                .Where(c => c.RecipeId == recipeId)
                .ToList();

            foreach (Comment comment in recipeComments)
            {
                Comment? existing = _commentRepo.Retrieve(comment.CommentId);
                if (existing == null) continue; 

                if (existing.ParentCommentId == null || existing.ParentCommentId == 0)
                {
                    DeleteCommentWithReplies(existing.CommentId);
                }
            }

           
            List<Comment> remainingComments = _commentRepo.RetrieveAll()
                .Where(c => c.RecipeId == recipeId)
                .ToList();

            foreach (Comment comment in remainingComments)
            {
                _commentRepo.Delete(comment.CommentId);
            }
        }

        public void RemoveAllCommentsByUserId(int userId)
        {
            List<Comment> userComments = _commentRepo.RetrieveAll()
                .Where(c => c.UserId == userId)
                .ToList();

            foreach (Comment comment in userComments)
            {

                Comment? existingComment = _commentRepo.Retrieve(comment.CommentId);
                if (existingComment == null) continue;

              
                if (existingComment.ParentCommentId == null || existingComment.ParentCommentId == 0)
                {
                    
                    DeleteCommentWithReplies(existingComment.CommentId);
                }
                else
                {
                    _commentRepo.Delete(existingComment.CommentId);
                }
            }
        }

        private void DeleteCommentWithReplies(int parentCommentId)
        {
            
            List<Comment> replies = _commentRepo.RetrieveAll()
                .Where(c => c.ParentCommentId == parentCommentId)
                .ToList();

            
            foreach (Comment reply in replies)
            {
                DeleteCommentWithReplies(reply.CommentId);
            }

            
            _commentRepo.Delete(parentCommentId);
        }

        public int GetCommentCount(int recipeId)
        {
            return _commentRepo.RetrieveAll()
                .Count(c => c.RecipeId == recipeId);
        }

       
        public Dictionary<int, int> GetAllCommentCounts()
        {
            return _commentRepo.RetrieveAll()
                .GroupBy(c => c.RecipeId)
                .ToDictionary(g => g.Key, g => g.Count());
        }

    }


}
