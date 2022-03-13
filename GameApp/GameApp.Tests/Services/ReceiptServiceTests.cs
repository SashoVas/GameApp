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
            for (int i = 0; i < 16; i++)
            {
                var receipt = new Receipt
                {
                    Id = i.ToString(),
                    User = i % 2 == 0 ? user1 : user2,
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

            var receiptService = new ReceiptService(null,receipts,null);
            var result = (await receiptService.GetAll(userId)).ToList();

            var actualData = receipts.All().Where(r => r.UserId == userId).ToList();

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

            var receiptService = new ReceiptService(null, receipts, null);
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
            var receiptService = new ReceiptService(null, receipts,null);

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
            var receiptService = new ReceiptService(null, receipts, null);

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
            var userManagerMock = new Mock<UserManager<User>>();
            var store = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            var user = context.Users.SingleOrDefault(u => u.Id == userId);
            userManager.Setup(u => u.FindByIdAsync(userId)).Returns(async() => user);

            var cardServiceMock = new CardService(new Repository<Card>(context),null);
            var userService = new UserService(userManager.Object,null,null);
            var receiptService = new ReceiptService(userService, receipts, cardServiceMock);
            Assert.True(await receiptService.CreateReceipt(userId,new List<UserGame>(),"Card1"));
            await context.SaveChangesAsync();

            var result = receipts.All().Last();

            var actual =new Receipt
            {
                UserId=userId,
            };

            Assert.Equal(result.UserId, actual.UserId);

        }
        [Theory]
        [InlineData(null,"Card1")]
        [InlineData("1",null)]
        public async Task TestCreateReceiptWithImproperDataShouldReturnNull(string userId,string cardId)
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var receipts = new Repository<Receipt>(context);
            var userManagerMock = new Mock<UserManager<User>>();
            var store = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            var user = context.Users.SingleOrDefault(u => u.Id == userId);
            userManager.Setup(u => u.FindByIdAsync(userId)).Returns(async () => user);

            var cardServiceMock = new CardService(new Repository<Card>(context), null);
            var userService = new UserService(userManager.Object,null,null);
            var receiptService = new ReceiptService(userService, receipts, cardServiceMock);
            Assert.False(await receiptService.CreateReceipt(userId, new List<UserGame>(), cardId));

        }
    }
}
