using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using GameApp.Services.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services
{
    public class GameService : IGameService
    {
        private readonly IRepository<Game> games;
        private readonly IRepository<UserGame> userGames;
        private readonly IRepository<Genre> genres;
        private readonly ShoppingCart shoppingCart;
        private readonly UserManager<User> userManager;
        private readonly IHostingEnvironment environment;

        public GameService(IRepository<Game> games, IRepository<UserGame> userGames,
            ShoppingCart shoppingCart, UserManager<User> userManager,IRepository<Genre> genres, IHostingEnvironment environment)
        {
            this.games = games;
            this.userGames = userGames;
            this.shoppingCart = shoppingCart;
            this.userManager = userManager;
            this.genres = genres;
            this.environment = environment;
        }

        public async Task<bool> BuyItems(string userId)
        {
            var cartItems = await shoppingCart
                .GetCartItems()
                .ToListAsync();
            
            var user=await userManager
                .FindByIdAsync(userId);

            cartItems
                .ForEach(async item =>await userGames
                .AddAsync(new UserGame 
                { 
                    Game=item,
                    User=user
                }));

            await userGames.SaveChangesAsync();

                return true;
        }

        public async Task<int> Create(string name, decimal price, string description, IEnumerable<string> newGenres, IFormFile image)
        {
            var game = new Game {
                Name = name,
                Price = price,
                Description = description,
                Genres= newGenres.Select(g=> new GameGenre {Genre= genres.All().SingleOrDefault(og=>og.Name==g) }).ToList()
            };
            
            if (image==null)
            {
                game.ImageUrl = "User.png";
            }
            else
            {
                var imgUrl = Guid.NewGuid().ToString();
                using (var path = System.IO.File.OpenWrite(environment.WebRootPath+"/Files/"+imgUrl+".png"))
                {
                    await image.CopyToAsync(path);
                }
                game.ImageUrl = imgUrl + ".png";
            }
            

            await games.AddAsync(game);
            await games.SaveChangesAsync();
            return game.Id;
        }

        public IEnumerable<AllGamesServiceListingModel> GetAll(int page, string gameName)
        {
            return games
                .All()
                .Take((page + 1) * 10)
                .Where(g=>g.Name
                .ToLower()
                .Contains(gameName))
                .Select(g => new AllGamesServiceListingModel
            { 
                Name=g.Name,
                Price=g.Price,
                Genres=g.Genres.Select(gg=>gg.Genre.Name).ToList(),
                ImgUrl=g.ImageUrl
            }).ToList();
        }

        public GameServiceListingModel GetGame(string name)
        {
            var game = games.All().FirstOrDefault(g => g.Name == name);
            if (game == null)
            {
                return null;
            }    
            return new GameServiceListingModel
            {
                Id=game.Id,
                Description = game.Description,
                Name = game.Name,
                Price = game.Price,
                ImgUrl=game.ImageUrl,
                Genres=game.Genres.Select(gg=>gg.Genre.Name)
            };
        }

        public async Task<IEnumerable<AllGamesServiceListingModel>> MyGames(string id)
        {
            return await userGames
                .All()
                .Where(ug => ug.UserId == id)
                .Select(ug => new AllGamesServiceListingModel
                {
                    Name = ug.Game.Name,
                    Price = ug.Game.Price,
                    Genres=null,
                    ImgUrl=ug.Game.ImageUrl
                }).ToArrayAsync();
        }
    }
}
