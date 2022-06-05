using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GameApp.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRepository<Review> reviews;
        private readonly IGameService gameService;

        public ReviewService(IRepository<Review> reviews, IGameService gameService)
        {
            this.reviews = reviews;
            this.gameService = gameService;
        }
        
        public async Task<bool> Rate(string gameName, int points, string userId)
        {
            var oldReview = await reviews.All()
                .FirstOrDefaultAsync(r => r.UserId == userId && r.Game.Name == gameName);
            if (oldReview != null)
            {
                return await ChangeRaing(oldReview,points);
            }
            var review = new Review
            {
                Score = points,
                ReviewDate = DateTime.UtcNow,
                UserId=userId
            };
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
