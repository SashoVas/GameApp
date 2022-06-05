using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using GameApp.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace GameApp.Services
{
    public class CommentsService : ICommentsService
    {
        private readonly IRepository<Comment> comments;
        public CommentsService(IRepository<Comment> comments) 
            => this.comments = comments;

        public async Task<bool> CommentExist(string id)
        => await comments.All().AnyAsync(c=>c.Id==id);

        public async Task<IEnumerable<CommentsServiceListingModel>> Create(int gameId, string commentConntents, string userId)
        {
            var comment = new Comment {
                Id=Guid.NewGuid().ToString(),
                Content=commentConntents,
                PostedOn=DateTime.UtcNow,
                GameId=gameId,
                UserId=userId
            };
            await comments.AddAsync(comment);
            await comments.SaveChangesAsync();
            return new List<CommentsServiceListingModel>{ new CommentsServiceListingModel
            {
                Contents = comment.Content,
                PostedOn = comment.PostedOn.ToString("yyyy,MM,dd"),
                CommentId = comment.Id,
                HasComments = comment.Comments.Count > 0 ? true : false
            }};
        }

        public async Task<IEnumerable<ReplyServiceListingModel>> CreateReply(int gameId, string commentConntents, string userId, string commentId)
        {
            var comment = new Comment
            {
                Id = Guid.NewGuid().ToString(),
                Content = commentConntents,
                PostedOn = DateTime.UtcNow,
                CommentedOnId=commentId,
                UserId=userId,
                GameId=gameId
            };
            await comments.AddAsync(comment);
            await comments.SaveChangesAsync();
            return  new List<ReplyServiceListingModel> { new ReplyServiceListingModel 
            {
                Content = comment.Content,
                CommentId = comment.Id,
                HasComments = comment.Comments.Count > 0 ? true : false
            } };
        }

        public async Task<IEnumerable<CommentsServiceListingModel>> LoadComments(int pageId, int gameId) 
            => await comments
                .All()
                .Where(c => c.GameId == gameId && c.CommentedOnId == null)
                .OrderByDescending(c => c.PostedOn)
                .Skip(pageId * 10)
                .Take(10)
                .Select(c => new CommentsServiceListingModel
                {
                    Username = c.User.UserName,
                    Contents = c.Content,
                    PostedOn = c.PostedOn.ToString("yyyy,MM,dd"),
                    CommentId = c.Id,
                    HasComments = c.Comments.Count > 0 ? true : false
                }).ToListAsync();

        public async Task<IEnumerable<ReplyServiceListingModel>> LoadReplies(string commentId) 
            => await comments
                .All()
                .Where(c => c.CommentedOnId == commentId)
                .OrderByDescending(c => c.PostedOn)
                .Select(c => new ReplyServiceListingModel
                {
                    Username = c.User.UserName,
                    Content = c.Content,
                    CommentId = c.Id,
                    HasComments = c.Comments.Count > 0 ? true : false
                }).ToListAsync();
    }
}
