using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services;
using GameApp.Tests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task TestAddToCartShouldReturnTrue()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<ShoppingCartGame>(context);
            var shoppingCart = new ShoppingCart(repo);
            shoppingCart.Id = "1";

            var gameService = new GameService(new Repository<Game>(context),null,null);
            var cardService = new CartService(shoppingCart, null,null, gameService, null);
            Assert.True(await cardService.AddToCart(1));
        }
        [Fact]
        public async Task TestAddToCartWithImproperDataShouldReturnFalse()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<ShoppingCartGame>(context);
            var shoppingCart = new ShoppingCart(repo);
            shoppingCart.Id = "1";

            var gameService = new GameService(new Repository<Game>(context), null, null);
            var cardService = new CartService(shoppingCart, null, null, gameService, null);
            Assert.False(await cardService.AddToCart(2));
        }
        [Fact]
        public async Task TestGetAllItemsShouldReturnAllItems()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<ShoppingCartGame>(context);
            var shoppingCart = new ShoppingCart(repo);
            shoppingCart.Id = "1";

            var cardService = new CartService(shoppingCart, null, null, null, null);
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
            var cardService = new CartService(shoppingCart, null, null, gameService, null);
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
            var cardService = new CartService(shoppingCart, null, null, gameService, null);
            Assert.False(await cardService.RemoveFromCart(1));
        }
        [Fact]
        public async Task TestBuyItemsShouldBuyItem()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var store = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new User 
            {
                Id="newUser",
                UserName="newUser"
            };
            userManagerMock.Setup(u => u.FindByIdAsync("newUser")).Returns(async()=>user);
            var repo = new Repository<ShoppingCartGame>(context);
            var shoppingCart = new ShoppingCart(repo);
            shoppingCart.Id = "1";
            var userService = new UserService(userManagerMock.Object, null, new Repository<User>(context));
            var cardService = new CardService(new Repository<Card>(context),null);
            var receiptService = new ReceiptService(userService,new Repository<Receipt>(context),cardService);
            
            var cartService = new CartService(shoppingCart, new Repository<UserGame>(context), userManagerMock.Object, null,receiptService );

            
            var card = new Card 
            {
                Id="newCard",
                FirstName="newCard",
                User=user,
                Address = "newCard",
                CardNumber = "newCard",
                ExpirationDate = DateTime.MinValue,
                City = "newCard",
                Country = "newCard",
                ZipCode = "newCard",
                LastName = "newCard",
                PhoneNumber = "newCard",
                CardType =CardType.MasterCard,
            };
            await context.Cards.AddAsync(card);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            Assert.True(await cartService.BuyItems("newUser","newCard"));
        }
        [Theory]
        [InlineData("newUser","DoNotExist")]
        [InlineData("DoNotExist", "newCard")]
        public async Task TestBuyItemsWithImproperDataShouldReturnFalse(string userId,string cardId)
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var store = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new User
            {
                Id = "newUser",
                UserName = "newUser"
            };
            var card = new Card
            {
                Id = "newCard",
                FirstName = "newCard",
                User = user,
                Address = "newCard",
                CardNumber = "newCard",
                ExpirationDate = DateTime.MinValue,
                City = "newCard",
                Country = "newCard",
                ZipCode = "newCard",
                LastName = "newCard",
                PhoneNumber = "newCard",
                CardType = CardType.MasterCard,
            };
            await context.Cards.AddAsync(card);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            userManagerMock.Setup(u => u.FindByIdAsync(userId)).Returns(async () =>await context.Users.FirstOrDefaultAsync(u=>u.Id==userId));
            var repo = new Repository<ShoppingCartGame>(context);
            var shoppingCart = new ShoppingCart(repo);
            shoppingCart.Id = "1";
            var userService = new UserService(userManagerMock.Object, null, new Repository<User>(context));
            var cardService = new CardService(new Repository<Card>(context), null);
            var receiptService = new ReceiptService(userService, new Repository<Receipt>(context), cardService);

            var cartService = new CartService(shoppingCart, new Repository<UserGame>(context), userManagerMock.Object, null, receiptService);


            
            Assert.False(await cartService.BuyItems(userId, cardId));
        }

    }
}
