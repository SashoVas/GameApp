using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using GameApp.Services.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services
{
    public class CommentsService : ICommentsService
    {
        private readonly IRepository<Comment> comments;
        private readonly IGameService gameService;
        private readonly IUserService userService;
        public CommentsService(IRepository<Comment> comments, IUserService userService, IGameService gameService)
        {
            this.comments = comments;
            this.userService = userService;
            this.gameService = gameService;
        }
        public async Task<IEnumerable<CommentsServiceListingModel>> Create(int gameId, string commentConntents, string userId)
        {
            var comment = new Comment {
                Id=Guid.NewGuid().ToString(),
                Content=commentConntents,
                PostedOn=DateTime.UtcNow,
            };
            var hasUser =await userService.SetUsersToComment(comment, userId);
            if (!hasUser)
            {
                throw new ArgumentException();
            }
            var hasGame=await gameService.SetGameById(comment, gameId);
            
            if (!hasGame)
            {
                throw new ArgumentException();
            }
            await comments.AddAsync(comment);
            await comments.SaveChangesAsync();
            return new List<CommentsServiceListingModel>{ new CommentsServiceListingModel
            {
                Username = comment.User.UserName,
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
            };
            var commentedOn = await comments.All().SingleOrDefaultAsync(c => c.Id == commentId);
            if (commentedOn == null)
            {
                throw new ArgumentException();
            }
            comment.CommentedOn = commentedOn;
            var hasUser = await userService.SetUsersToComment(comment, userId);
            if (!hasUser)
            {
                throw new ArgumentException();
            }
            var hasGame=await gameService.SetGameById(comment, gameId);
            if (!hasGame)
            {
                throw new ArgumentException();
            }
            await comments.AddAsync(comment);
            await comments.SaveChangesAsync();
            return  new List<ReplyServiceListingModel> { new ReplyServiceListingModel 
            {
                Username = comment.User.UserName,
                Content = comment.Content,
                CommentId = comment.Id,
                HasComments = comment.Comments.Count > 0 ? true : false
            } };
        }

        public async Task<IEnumerable<CommentsServiceListingModel>> LoadComments(int pageId,int gameId)
        {
            
            var commentsList =await comments
                .All()
                .Where(c => c.GameId == gameId && c.CommentedOnId==null)
                .OrderByDescending(c=>c.PostedOn)
                .Skip(pageId*10)
                .Take(10)
                .Select(c => new CommentsServiceListingModel
                {
                    Username=c.User.UserName,
                    Contents=c.Content,
                    PostedOn=c.PostedOn.ToString("yyyy,MM,dd"),
                    CommentId=c.Id,
                    HasComments = c.Comments.Count > 0 ? true : false
                }).ToListAsync();
            return commentsList;
        }

        public async Task<IEnumerable<ReplyServiceListingModel>> LoadReplies(string commentId)
        {
            return await comments
                .All()
                .Where(c => c.CommentedOnId == commentId)
                .OrderByDescending(c=>c.PostedOn)
                .Select(c => new ReplyServiceListingModel
                {
                    Username = c.User.UserName,
                    Content = c.Content,
                    CommentId = c.Id,
                    HasComments = c.Comments.Count > 0 ? true : false
                }).ToListAsync();
        }
    }
}
