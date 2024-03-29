﻿using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services;
using GameApp.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GameApp.Tests.Services
{
    public class ReceiptServiceTests
    {
        private List<Receipt> GetDummyData()
        {
            var user1 = new User
            {
                UserName = "1",
                Id = "1"
            };

            var user2 = new User
            { 
                UserName= "2",
                Id = "2",
            };
            var receipts = new List<Receipt>();
            for (int i = 1; i < 16; i++)
            {
                var receipt = new Receipt
                {
                    Id = i.ToString(),
                    UserGames=new List< ReceiptUserGame> 
                    { 
                        new ReceiptUserGame
                        { 
                            Id=i.ToString(),
                            UserGame=new UserGame
                            { 
                                User=i % 2 == 0 ? user1 : user2,
                                Game=new Game
                                { 
                                    Name=i.ToString(),
                                    Id=i,
                                    Description=i.ToString()
                                }
                            }
                        }
                    },
                    Card = new Card
                    {
                        Id = "Card" + i.ToString(),
                        Address = "Card" + i.ToString(),
                        CardNumber = "Card" + i.ToString(),
                        CardType = CardType.MasterCard,
                        City = "Card" + i.ToString(),
                        Country = "Card" + i.ToString(),
                        ExpirationDate = DateTime.UtcNow,
                        FirstName="Card"+i.ToString(),
                        LastName="Card"+i.ToString(),
                        PhoneNumber="Card"+i.ToString(),
                        ZipCode="Card"+i.ToString(),
                        User = i % 2 == 0 ? user1 : user2,
                    }
                };
                receipts.Add(receipt);
            }

            return receipts;
        }
        private async Task SeedData(GameAppDbContext context) 
        {
            context.Receipts.AddRange(GetDummyData());
            await context.SaveChangesAsync();
        }
        [Fact]
        
        public async Task TestGetAllReceiptsWithProperDataShouldReturnAllReceipts() 
        {
            var userId = "1";
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var receipts = new Repository<Receipt>(context);

            var receiptService = new ReceiptService(receipts);
            var result = (await receiptService.GetAll(userId)).ToList();

            var actualData = receipts.All().Where(r => r.UserGames.FirstOrDefault().UserGame.UserId == userId).ToList();

            for (int i = 0; i < result.Count; i++)
            {
                Assert.Equal(result[i].Id,actualData[i].Id);
            }
        }
        [Theory]
        [InlineData("-1")]
        [InlineData(null)]
        public async Task TestGetAllReceiptsWithImproperData(string userId)
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var receipts = new Repository<Receipt>(context);

            var receiptService = new ReceiptService(receipts);
            var result = (await receiptService.GetAll(userId)).ToList();
            Assert.Equal(result.Count(),0);
        }
        [Theory]
        [InlineData("1")]
        [InlineData("5")]

        public async Task TestGetReceiptShouldReturnReceipt(string receiptId)
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var receipts = new Repository<Receipt>(context);
            var receiptService = new ReceiptService(receipts);

            var result = await receiptService.GetReceipt(receiptId);

            var actualData = receipts.All().SingleOrDefault(r=>r.Id== receiptId);

            Assert.Equal(result.Id, actualData.Id);
        }
        [Theory]
        [InlineData("-1")]
        [InlineData("")]
        [InlineData(null)]
        public async Task TestGetReceiptWithImproperDataShouldReturnNull(string receiptId)
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var receipts = new Repository<Receipt>(context);
            var receiptService = new ReceiptService(receipts);

            var result = await receiptService.GetReceipt(receiptId);

            Assert.Null(result);
        }
        [Theory]
        [InlineData("1")]
        public async Task TestCreateReceiptShouldCreateReceipt(string userId)
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var receipts = new Repository<Receipt>(context);
            var receiptService = new ReceiptService( receipts);
            Assert.True(await receiptService.CreateReceipt(userId,new List<UserGame> 
            { 
                new UserGame 
                {
                    Id=100,
                    UserId=userId,
                    Game=new Game
                    {
                        Name="newGame",
                        Id=100,
                        Description="newGame"
                    }
                }
            }
            ,"Card1",ReceiptType.Purchase));
            await context.SaveChangesAsync();

            var result = receipts.All().Last();

            Assert.Equal(result.UserGames.FirstOrDefault().UserGame.UserId, userId);

        }
    }
}
