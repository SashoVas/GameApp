using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services;
using GameApp.Services.Contracts;
using GameApp.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GameApp.Tests.Services
{
    public class UserGameServiceTests
    {
        private List<UserGame> GetDummyData()
        {
            var user1 = new User
            {
                Id = "1",
                UserName="User1"
            };
            var user2 = new User
            {
                Id = "2",
                UserName = "User2"
            };

            var data=new List<UserGame>();
            for (int i = 1; i < 16; i++)
            {
                var userGame = new UserGame
                {
                    User = i % 2 == 1 ? user1 : user2,
                    Date = DateTime.Now.AddDays(2 - i),
                    Game = new Game
                    {
                        Id = i,
                        Name = "Game" + i,
                        ImageUrl = "Game" + i,
                        ReleaseDate = DateTime.Now.AddDays(-i),
                        Description="Description"+i
                    },
                    Receipts=new List<ReceiptUserGame>
                    {
                        new ReceiptUserGame
                        { 
                            Id=i.ToString(),
                            Receipt= new Receipt 
                            {
                                Id=i.ToString(),
                                CardId=i.ToString(),
                            }
                        }
                    }
                   
                };

                data.Add(userGame);
            }

            return data;
        }
        private async Task SeedData(GameAppDbContext dbContext)
        {
            await dbContext.AddRangeAsync(GetDummyData());
            await dbContext.SaveChangesAsync();
        }
        [Fact]
        public async Task TestGetGameForRefundShouldReturnGames()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<UserGame>(context);

            var userGameService = new UserGameService(repo,null);

            var result =(await userGameService.GetGameForRefund("1"))
                .ToList();

            var actual =await repo
                .All()
                .Where(ug => ug.UserId == "1" && ug.Date > DateTime.Now.AddDays(-3))
                .ToListAsync();

            for (int i = 0; i < result.Count(); i++)
            {
                Assert.Equal(result[i].Name, actual[i].Game.Name);
                Assert.Equal(result[i].GameId, actual[i].Game.Id);
                Assert.Equal(result[i].ReleaseDate, actual[i].Game.ReleaseDate);
                Assert.Equal(result[i].IMG, actual[i].Game.ImageUrl);
            }
        }
        [Fact]
        public async Task TestGetGameForRefundWithImproperDataShouldReturnEmpty()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<UserGame>(context);
            var userGameService = new UserGameService(repo,null);

            Assert.Empty(await userGameService.GetGameForRefund("3"));

        }
        [Fact]
        public async Task TestRefundGameShouldRefundGame()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<UserGame>(context);

            var receiptService = new Mock<IReceiptService>();
            receiptService.Setup(rs=>rs.CreateReceipt(It.IsAny<string>(), It.IsAny<List<UserGame>>(), It.IsAny<string>(),ReceiptType.Refund))
                .Returns(async()=>true);
            var userGameService = new UserGameService(repo, receiptService.Object);

            var result = await repo.All().FirstOrDefaultAsync(ug => ug.UserId == "1" && ug.GameId == 1);
            Assert.False(result.IsRefunded);
            Assert.True(await userGameService.RefundGame(1,"1"));
            result = await repo.All().FirstOrDefaultAsync(ug => ug.UserId == "1" && ug.GameId == 1);
            Assert.True(result.IsRefunded);

        }
        [Theory]
        [InlineData(1,"3")]
        [InlineData(2,"1")]
        public async Task TestRefundGameWithImproperDataShouldReturnFalse(int gameId,string userId)
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<UserGame>(context);
            var userGameService = new UserGameService(repo,null);

            Assert.False(await userGameService.RefundGame(gameId, userId));
        }

    }
}
