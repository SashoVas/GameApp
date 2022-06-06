using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services;
using GameApp.Services.Contracts;
using GameApp.Tests.Infrastructure;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GameApp.Tests.Services
{
    public class ReviewServiceTests
    {
        private List<Review> GetDummyData()
        {
            var reviews = new List<Review>();
            var user1 = new User
            {
                Id = "1",
                UserName="Username1"
            };
            var user2 = new User
            {
                Id = "2",
                UserName="Username2"
            };
            for (int i = 1; i < 16; i++)
            {
                var review = new Review
                {
                    Score = i < 10 ? i : i / 2,
                    Game = new Game
                    {
                        Id = i,
                        Name = "Game" + i.ToString(),
                        Description = "Description" + i.ToString(),
                        Price = 50 * i
                    },
                    User = i % 2 == 0 ? user1 : user2
                };
                reviews.Add(review);
            }

            return reviews;
        }
        private async Task SeedData(GameAppDbContext context)
        {
            context.Reviews.AddRange(GetDummyData());
            await context.SaveChangesAsync();
        }
        [Fact]
        public async Task TestRate()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Review>(context);
            var gameServiceMock = new Mock<IGameService>();
            gameServiceMock
                .Setup(g => g.SetGameByName(It.IsAny<Review>(), It.IsAny<string>()))
                .Returns(async (Review review,string gameName) =>
                {
                    review.GameId = 1;
                    return true;
                });
            var reviewService = new ReviewService(repo, gameServiceMock.Object);

            Assert.True(await reviewService.Rate("Game1",5,"1"));

            var game = context.Games.SingleOrDefault(g=>g.Name== "Game1");
            var result = repo
                .All()
                .Last();
            var actual = new Review 
            { 
                GameId=game.Id,
                UserId="1",
                Score=5,
                Game=game,
            };

            Assert.Equal(result.Score,actual.Score);
            Assert.Equal(result.UserId, actual.UserId);
            Assert.Equal(result.GameId, actual.GameId);
        }
    }
}
