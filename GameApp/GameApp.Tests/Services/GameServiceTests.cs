﻿using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services;
using GameApp.Services.Contracts;
using GameApp.Services.Models;
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
    
    public class GameServiceTests
    {
        private List<Game>GetDummyData()
        {
            var games = new List<Game>();

            for (int i = 0; i < 16; i++)
            {
                var gameGenres=new List<GameGenre>();
                var userGames=new List<UserGame>();
                var game = new Game
                {
                    Name = "Game" + i.ToString(),
                    Description = "Description" + i.ToString(),
                    Price = 324,
                    ImageUrl = "User.png",
                    
                };
                if (i%2==0)
                {
                    userGames.Add(new UserGame { User = new User { UserName = "UserB" }, Receipts=new List<ReceiptUserGame>() });
                    gameGenres.Add(new GameGenre { Genre = new Genre { Name = "Comedy" } });
                }
                userGames.Add(new UserGame { User = new User {  UserName = "UserA"  }, Receipts = new List<ReceiptUserGame>() } );
                gameGenres.Add(new GameGenre { Genre = new Genre { Name= "Action" } });
                game.Genres = gameGenres;
                game.Users = userGames;
                games.Add(game);
            }
            return games;
        }
        private async Task SeedData(GameAppDbContext context) 
        {
            context.Games.AddRange(GetDummyData());
            await context.SaveChangesAsync();
        }
        [Theory]
        [InlineData(0,"11","Action","UserA")]
        [InlineData(0,"ame","Comedy",null)]
        [InlineData(1, "Game", null, "UserA")]
        [InlineData(1, "", null, "UserB")]
        [InlineData(0, "Game1", "Comedy", "UserB")]
        public async Task TestGetAllGames_ShouldReturnAllGames(int page, string gameName, string genre, string username)
        {
            var context = GameAppDbContextFactory.InitializeContext();
            var games = new Repository<Game>(context);
            var gameService = new GameService(games, null, null);
            await SeedData(context);

            var a =await gameService.GetAll(page, gameName, genre, username);
            var result = a.ToList();
            var dummyData = GetDummyData()
                .Where(g=>g.Name.ToLower().Contains(gameName.ToLower()))
                .Skip(page * 10);
            if (genre!=null)
            {
                dummyData = dummyData.Where(g => g.Genres.Any(gg => gg.Genre.Name == genre));
            }
            if (username != null)
            {
                dummyData = dummyData.Where(g => g.Users.Any(ug => ug.User.UserName == username));
            }
            var actualData= dummyData.ToList();
            for (int i = 0; i < result.Count(); i++)
            {
                
                Assert.Equal(result[i].Name, actualData[i].Name);
                Assert.Equal(result[i].Price, actualData[i].Price);
                Assert.Equal(result[i].ImgUrl, actualData[i].ImageUrl);
                var resultGenres = result[i].Genres.ToList();
                var actualGenres=actualData[i].Genres.ToList(); 
                for (int j = 0; j < resultGenres.Count(); j++)
                {
                    Assert.Equal(resultGenres[j], actualGenres[j].Genre.Name);
                }

            }

        }
        [Theory]
        [InlineData("Game1","UserA")]
        [InlineData("Game11",null)]
        public async Task TestGetGameShouldReturnGame(string gameName,string username)
        {
            var context = GameAppDbContextFactory.InitializeContext();
            var games = new Repository<Game>(context);
            var gameService = new GameService(games, null, null);
            await SeedData(context);
            var game = await gameService.GetGame(gameName, username);
            var actualData = GetDummyData().SingleOrDefault(g=>g.Name== gameName);

            Assert.Equal(game.Name,actualData.Name);
            Assert.Equal(game.ImgUrl,actualData.ImageUrl);
            Assert.Equal(game.Price,actualData.Price);
            Assert.Equal(game.Description,actualData.Description);
            Assert.Equal(game.ReleaseDate,actualData.ReleaseDate);
            var resultGenres = game.Genres.ToList();
            var actualGenres = actualData.Genres.ToList();
            for (int j = 0; j < resultGenres.Count(); j++)
            {
                Assert.Equal(resultGenres[j], actualGenres[j].Genre.Name);
            }
        }
        [Fact]
        public async Task TestGetGameWithWrongValueShouldReturnNull()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            var games = new Repository<Game>(context);
            var gameService = new GameService(games, null, null);
            await SeedData(context);
            var game = await gameService.GetGame(null, null);
            Assert.Null(game);
        }
        [Fact]
        public async Task TestCreateGame()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            var games = new Repository<Game>(context);
            var genreService = new Mock<IGenreService>();
            var gameService = new GameService(games, genreService.Object, null);
            var id=await gameService.Create("TestGame",30,"smt",DateTime.MinValue,new List<string>(),null,null);
            var result = games.All().Last();

            var actualGame = new Game 
            {
                Id=id,
                Name="TestGame",
                Price=30,
                Description="smt",
                ReleaseDate=DateTime.MinValue,
                ImageUrl="User.png"
            };

            Assert.Equal(result.Id, actualGame.Id);
            Assert.Equal(result.Name, actualGame.Name);
            Assert.Equal(result.Price, actualGame.Price);
            Assert.Equal(result.Description, actualGame.Description);
            Assert.Equal(result.ReleaseDate, actualGame.ReleaseDate);
            Assert.Equal(result.ImageUrl, actualGame.ImageUrl);

        }


        [Fact]
        public async Task TestGetUpcomingGames()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            var games = new Repository<Game>(context);
            await SeedData(context);
            var gameService = new GameService(games, null, null);

            for (int i = 1; i < 9; i++)
            {
                var game = new Game
                {
                    Name = "UpcomingGame" + i.ToString(),
                    Description = "UpcomingGame" + i.ToString(),
                    Id = i + 60,
                    Price = 60,
                    ReleaseDate = DateTime.UtcNow.AddDays(50 + i)
                };

                await games.AddAsync(game);
            }
            await context.SaveChangesAsync();

            var result =await gameService.GetUpcomingGames();

            var actual =await games
                .All()
                .Where(g=>g.ReleaseDate>DateTime.UtcNow)
                .ToListAsync();

            for (int i = 0; i < result.Length; i++)
            {
                Assert.Equal(result[i].Name,actual[i].Name);
                Assert.Equal(result[i].ImgUrl, actual[i].ImageUrl);
            }
        }
        [Fact]
        public async Task TestIsUpcomingShouldReturnTrue()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            var games = new Repository<Game>(context);
            await SeedData(context);
            var gameService = new GameService(games, null, null);

            var game = new Game
            {
                Name = "UpcomingGame" ,
                Description = "UpcomingGame" ,
                Id =  60,
                Price = 60,
                ReleaseDate = DateTime.UtcNow.AddDays(50 )
            };

            await games.AddAsync(game);
            await context.SaveChangesAsync();

            Assert.True(await gameService.IsUpcoming(60));
        }
        [Fact]
        public async Task TestIsIsUpcomingWithImproperDataShouldReturnFalse()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            var games = new Repository<Game>(context);
            await SeedData(context);
            var gameService = new GameService(games, null, null);

            Assert.False(await gameService.IsUpcoming(5));
        }
        [Fact]
        public async Task TestDeleateGameShouldDelateGame()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            var games = new Repository<Game>(context);
            await SeedData(context);
            var gameService = new GameService(games, null, null);
            Assert.True(await games.All().AnyAsync(g=>g.Name=="Game1"));
            context.ChangeTracker.Clear();

            Assert.True(await gameService.Deleate("Game1"));
            Assert.False(await games.All().AnyAsync(g => g.Name == "Game1"));

        }
        [Fact]
        public async Task TestGetTopRankedGames()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            var games = new Repository<Game>(context);
            await SeedData(context);
            var gameService = new GameService(games, null, null);

            var result =await gameService.GetTopRankedGames();

            var actual =await games.All()
                .OrderByDescending(g => g.Reviews.Sum(r => r.Score) / (g.Reviews.Count() > 0 ? g.Reviews.Count() : 1))
                .Take(15)
                .Select(g => new PopularGamesServiceListingModel
                {
                    ImgUrl = g.ImageUrl,
                    Name = g.Name
                }).ToArrayAsync();
            for (int i = 0; i < result.Count(); i++)
            {
                Assert.Equal(actual[i].Name, result[i].Name);
                Assert.Equal(actual[i].ImgUrl, result[i].ImgUrl);
            }
        }
        [Fact]
        public async Task TestGameExist()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            var games = new Repository<Game>(context);
            await SeedData(context);
            var gameService = new GameService(games, null, null);
            Assert.True(await gameService.GameExist(1));
            Assert.False(await gameService.GameExist(100));
        }
        [Fact]
        public async Task TestGameExistByName()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            var games = new Repository<Game>(context);
            await SeedData(context);
            var gameService = new GameService(games, null, null);
            Assert.True(await gameService.GameExistByName("Game1"));
            Assert.False(await gameService.GameExistByName("NoGame"));
        }
        [Fact]
        public async Task TestAddShoppingCartItem()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            var games = new Repository<Game>(context);
            await SeedData(context);
            var shoppingCart =new ShoppingCart(new Repository<ShoppingCartGame>(context));
            shoppingCart.Id = "1";
            var gameService = new GameService(games, null, null);
            Assert.True(await gameService.AddShoppingCartItem(shoppingCart,1));
            Assert.False(await gameService.AddShoppingCartItem(shoppingCart,100));
        }
    }
}
