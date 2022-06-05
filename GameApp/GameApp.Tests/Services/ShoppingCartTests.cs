using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GameApp.Tests.Services
{
    public class ShoppingCartTests
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
            context.ShoppingCartGames.AddRange(GetDummyData());
            await context.SaveChangesAsync();
        }
        [Fact]
        public async Task TestGetCartItemsShouldReturnAllCartGames()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);

            var repo = new Repository<ShoppingCartGame>(context);
            var shoppingCart = new ShoppingCart(repo);
            shoppingCart.Id = "1";

            var result = shoppingCart.GetCartItems().ToList();
            var actualData = repo
                .All()
                .Where(c => c.ShoppingCartId == "1")
                .Select(ci => ci.Game)
                .ToList();

            for (int i = 0; i < result.Count(); i++)
            {
                Assert.Equal(result[i].Id, actualData[i].Id);
                Assert.Equal(result[i].Name, actualData[i].Name);
                Assert.Equal(result[i].Price, actualData[i].Price);
                Assert.Equal(result[i].Description, actualData[i].Description);

            }
        }
        [Fact]
        public async Task TestClear()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);

            var repo = new Repository<ShoppingCartGame>(context);
            var shoppingCart = new ShoppingCart(repo);
            shoppingCart.Id = "1";
            Assert.NotEmpty(repo
                .All()
                .Where(c => c.ShoppingCartId == "1"));
            await shoppingCart.Clear();

            var result = repo
                .All().
                Where(c => c.ShoppingCartId == "1");
            Assert.Empty(result);
        }

        [Fact]
        public async Task TestRemoveFromCartShouldRemoveItems()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);

            var repo = new Repository<ShoppingCartGame>(context);
            var shoppingCart = new ShoppingCart(repo);
            shoppingCart.Id = "1";

            var game = context.Games.SingleOrDefault(g => g.Id == 2);

            Assert.True(await shoppingCart.RemoveFromCart(game.Id));
            var result = repo.All().FirstOrDefault(c => c.GameId == 2);
            Assert.Null(result);

        }
        [Theory]
        [InlineData("1",-2)]
        [InlineData("NotUser",2)]
        public async Task TestRemoveFromCartWithImproperDataShouldReturnNull(string shoppingCartId,int gameId)
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);

            var repo = new Repository<ShoppingCartGame>(context);
            var shoppingCart = new ShoppingCart(repo);
            shoppingCart.Id = shoppingCartId;

            Assert.False(await shoppingCart.RemoveFromCart(gameId));
        }

        [Fact]
        public async Task TestGetCartItemsWithImproperDataShouldReturnEmpty()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);

            var repo = new Repository<ShoppingCartGame>(context);
            var shoppingCart = new ShoppingCart(repo);
            shoppingCart.Id = null;

            var result = shoppingCart.GetCartItems().ToList();

            Assert.Empty(result);

        }
        [Fact]
        public async Task TestAddToCartShouldAddItem()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);

            var repo = new Repository<ShoppingCartGame>(context);
            var shoppingCart = new ShoppingCart(repo);
            shoppingCart.Id = "1";

            var game = new Game 
            {
                Id=20,
                Description="newGame",
                Name="newGame",
                Price=555
            };
            Assert.True(await shoppingCart.AddToCart(game));
            var result = repo
                .All()
                .Last();
            Assert.Equal(result.Game.Name, game.Name);
            Assert.Equal(result.Game.Id, game.Id);
            Assert.Equal(result.Game.Description, game.Description);
            Assert.Equal(result.Game.Price, game.Price);

        }
        [Fact]
        public async Task TestAddToCartWithImproperDataShouldReturnFalse()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);

            var repo = new Repository<ShoppingCartGame>(context);
            var shoppingCart = new ShoppingCart(repo);
            shoppingCart.Id = "1";

            var game =await context.Games.FirstAsync();
            Assert.True(await shoppingCart.AddToCart(game));
            Assert.False(await shoppingCart.AddToCart(game));
        }

    }
}
