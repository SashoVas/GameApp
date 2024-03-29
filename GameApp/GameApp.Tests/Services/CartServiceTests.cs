﻿using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services;
using GameApp.Services.Contracts;
using GameApp.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GameApp.Tests.Services
{
    public class CartServiceTests
    {
        private List<ShoppingCartGame> GetDummyData()
        {
            var shoppingCart = new List<ShoppingCartGame>();


            for (int i = 1; i < 16; i++)
            {
                var shoppingCartItem = new ShoppingCartGame
                {
                    Id = i.ToString(),
                    Game = new Game
                    {
                        Id = i,
                        Description = "Description" + i.ToString(),
                        Name = "GameName" + i.ToString(),
                        Price = i * 50
                    },
                    ShoppingCartId = i % 2 == 0 ? "1" : "2",
                    GameId = i
                };
                shoppingCart.Add(shoppingCartItem);
            }

            return shoppingCart;
        }

        private async Task SeedData(GameAppDbContext context)
        {
            await context.AddRangeAsync(GetDummyData());
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task TestGetAllItemsShouldReturnAllItems()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<ShoppingCartGame>(context);
            var shoppingCart = new ShoppingCart(repo);
            shoppingCart.Id = "1";

            var cardService = new CartService(shoppingCart, null, null, null);
            var actual=await repo.All().Where(sg=>sg.ShoppingCartId=="1").Select(sg=>sg.Game).ToListAsync();

            var result =(await cardService.GetAllItems()).ToList();

            for (int i = 0; i < result.Count(); i++)
            {
                Assert.Equal(result[i].GameName,actual[i].Name);
                Assert.Equal(result[i].Id, actual[i].Id);
                Assert.Equal(result[i].Price, actual[i].Price);
            }
        }
        [Fact]
        public async Task TestRemoveFromCartShouldRemoveItem()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<ShoppingCartGame>(context);
            var shoppingCart = new ShoppingCart(repo);
            shoppingCart.Id = "1";

            var gameService = new GameService(new Repository<Game>(context), null, null);
            var cardService = new CartService(shoppingCart, null, gameService, null);
            Assert.NotNull(await repo.All().FirstOrDefaultAsync(sg => sg.ShoppingCartId == "1" && sg.GameId == 2));
            Assert.True(await cardService.RemoveFromCart(2));
            Assert.Null(await repo.All().FirstOrDefaultAsync(sg => sg.ShoppingCartId == "1" && sg.GameId == 2));
        }

        [Fact]
        public async Task TestRemoveFromCartWithImproperDataShouldReturnFalse()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<ShoppingCartGame>(context);
            var shoppingCart = new ShoppingCart(repo);
            shoppingCart.Id = "1";

            var gameService = new GameService(new Repository<Game>(context), null, null);
            var cardService = new CartService(shoppingCart, null, gameService, null);
            Assert.False(await cardService.RemoveFromCart(1));
        }
        [Fact]
        public async Task TestBuyItemsShouldBuyItem()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var user = new User 
            {
                Id="newUser",
                UserName="newUser"
            };
            var repo = new Repository<ShoppingCartGame>(context);
            var shoppingCart = new ShoppingCart(repo);
            shoppingCart.Id = "1";
            var receiptServiceMock = new Mock<IReceiptService>();
            receiptServiceMock.Setup(rs => rs.CreateReceipt(It.IsAny<string>(), It.IsAny<List<UserGame>>(), It.IsAny<string>(), It.IsAny<ReceiptType>())).Returns(async () => true);

            var cartService = new CartService(shoppingCart, new Repository<UserGame>(context),  null, receiptServiceMock.Object);

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            Assert.NotEmpty(await shoppingCart.GetCartItems().ToListAsync());
            Assert.True(await cartService.BuyItems("newUser","doNotExist"));
            Assert.Empty(await shoppingCart.GetCartItems().ToListAsync());
        }
    }
}
