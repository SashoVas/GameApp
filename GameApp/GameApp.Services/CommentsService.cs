﻿using GameApp.Data.Models;
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
        private readonly IRepository<Game> games;
        private readonly UserManager<User> userManager;
        public CommentsService(IRepository<Comment> comments, IRepository<Game> games, UserManager<User> userManager)
        {
            this.comments = comments;
            this.games = games;
            this.userManager = userManager;
        }
        public async Task<bool> Create(int gameId, string commentConntents, string userId)
        {
            var comment = new Comment {
                Game=await games.All().SingleOrDefaultAsync(g=>g.Id==gameId),
                Content=commentConntents,
                PostedOn=DateTime.UtcNow,
                User=await userManager.FindByIdAsync(userId),
            };
            await comments.AddAsync(comment);
            await comments.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CommentsServiceListingModel>> LoadComments(int pageId,int gameId)
        {
            var commentsList =await comments
                .All()
                .Where(c => c.GameId == gameId)
                .Skip(pageId*10)
                .Take(10)
                .Select(c => new CommentsServiceListingModel
                {
                    Username=c.User.UserName,
                    Contents=c.Content,
                    PostedOn=c.PostedOn.ToString("yyyy,MM,dd")
                }).ToListAsync();
            return commentsList;
        }
    }
}
