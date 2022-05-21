using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public ReviewService(IRepository<Review> reviews, IGameService gameService, IUserService userService)
        {
            this.reviews = reviews;
            this.gameService = gameService;
            this.userService = userService;
        }
        
        public async Task<bool> Rate(string gameName, int points, string userId)
        {
            var oldReview = await reviews.All().FirstOrDefaultAsync(r => r.UserId == userId && r.Game.Name == gameName);
            if (oldReview != null)
            {
                return await ChangeRaing(oldReview,points);
            }
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
        private async Task<bool>ChangeRaing(Review review,int points)
        {
            review.Score = points;
            reviews.Update(review);
            await reviews.SaveChangesAsync();
            return true;
        }
    }
}
