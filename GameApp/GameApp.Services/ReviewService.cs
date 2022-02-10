using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRepository<Review> reviews;
        private readonly UserManager<User> users;
        private readonly IGameService gameService;

        public ReviewService(IRepository<Review> reviws, IGameService gameService, UserManager<User> users)
        {
            this.reviews = reviws;
            this.gameService = gameService;
            this.users = users;
        }

        public async Task<bool> Rate(string gameName, int points, string userId)
        {
            var review = new Review
            {
                Score = points,
                ReviewDate = DateTime.UtcNow,
                User = await users.FindByIdAsync(userId),

            };
            await gameService.SetGameByName(review, gameName);
            await reviews.AddAsync(review);
            await reviews.SaveChangesAsync();
            return true;
        }
    }
}
