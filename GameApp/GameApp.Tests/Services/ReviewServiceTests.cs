using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services;
using GameApp.Services.Contracts;
using GameApp.Tests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            var store = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            var user = context.Users.SingleOrDefault(u=>u.Id=="1");
            userManager.Setup(um=>um.FindByIdAsync("1")).Returns(async()=>user);

            var gameServiceMock = new Mock<GameService>(new Repository<Game>(context), null, null);

            var reviewService = new ReviewService(repo, gameServiceMock.Object, userManager.Object);

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
                User=user
            };

            Assert.Equal(result.Score,actual.Score);
            Assert.Equal(result.UserId, actual.UserId);
            Assert.Equal(result.GameId, actual.GameId);
            Assert.Equal(result.User.UserName, actual.User.UserName);
            Assert.Equal(result.Game.Name, actual.Game.Name);
            Assert.Equal(result.Game.Price, actual.Game.Price);
            Assert.Equal(result.Game.Description, actual.Game.Description);

        }

    }
}
