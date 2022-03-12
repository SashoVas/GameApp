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
        private readonly IUserService userService;
        private readonly IGameService gameService;

        public ReviewService(IRepository<Review> reviws, IGameService gameService, IUserService userService)
        {
            this.reviews = reviws;
            this.gameService = gameService;
            this.userService = userService;
        }

        public async Task<bool> Rate(string gameName, int points, string userId)
        {
            var review = new Review
            {
                Score = points,
                ReviewDate = DateTime.UtcNow,
            };
            var hasUser = await userService.SetUsersToReview(review,userId);
            if (!hasUser)
            {
                return false;
            }
            var hasGame=await gameService.SetGameByName(review, gameName);
            if (!hasGame)
            {
                return false;
            }
            await reviews.AddAsync(review);
            await reviews.SaveChangesAsync();
            return true;
        }
    }
}
