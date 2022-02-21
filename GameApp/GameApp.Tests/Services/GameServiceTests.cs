using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services;
using GameApp.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    userGames.Add(new UserGame { User = new User { UserName = "UserB" }, ReceiptId = "" });
                    gameGenres.Add(new GameGenre { Genre = new Genre { Name = "Comedy" } });
                }
                userGames.Add(new UserGame { User = new User {  UserName = "UserA"  },ReceiptId="" } );
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


    }
}
