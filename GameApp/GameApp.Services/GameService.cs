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
            await shoppingCart.Clear();
            

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

        public async Task< GameServiceListingModel> GetGame(string name,string userId)
        {
            var game =await games
                .All()
                .Include(g=>g.Users)
                .Include(g=>g.Genres)
                .ThenInclude(g=>g.Genre)
                .SingleOrDefaultAsync(g => g.Name == name);
            

            if (game==null)
            {
                return null;
            }
            var model= new GameServiceListingModel
            {
                Id=game.Id,
                Description = game.Description,
                Name = game.Name,
                Price = game.Price,
                ImgUrl=game.ImageUrl,
                Genres=game.Genres.Select(gg=>gg.Genre.Name),
                Users=game.Users.Count(),
                GameRating=game.RatingSum/((game.Users.Count()>0)?game.Users.Count():1),
                ReleaseDate=game.ReleaseDate
               
            };
            model.Rank = games
                .All()
                .Where(g => g.RatingSum / ((g.Users.Count() > 0) ? g.Users.Count() : 1) > model.GameRating)
                .Count()+1;

            //this game rank in last 30 days
            var dateDeadline = DateTime.UtcNow.AddDays(-30);
            var myPopularityRating = game.Users.Where(ug => ug.ReviewGamePosted > dateDeadline).Sum(ug => ug.Rating) / ((game.Users.Count() > 0) ? game.Users.Count() : 1);
            model.Popularity = games
            .All()
            .Select(g=>g.Users.Where(ug => ug.ReviewGamePosted > dateDeadline).Sum(ug => ug.Rating) / ((g.Users.Count() > 0) ? g.Users.Count() : 1))
            .Where(s=>s> myPopularityRating)
            .Count()+1;


            var usergame =await userGames
                .All()
                .SingleOrDefaultAsync(ug => ug.UserId == userId 
                && ug.GameId == game.Id);
            if (usergame!=null)
            {
                model.UserRating = usergame.Rating;
                model.HaveGame = true;
            }

            return model;
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

        public async Task<bool> Rate(string gameName, int points, string userId)
        {
            var model = await userGames
                .All()
                .Include(ug=>ug.Game)
                .SingleOrDefaultAsync(ug => ug.UserId == userId
                && ug.Game.Name == gameName);
            
            model.Game.RatingSum -= model.Rating;
            model.Game.RatingSum += points;
            model.Rating = points;
            model.ReviewGamePosted = DateTime.UtcNow;
            userGames.Update(model);
            await userGames.SaveChangesAsync();
            return true;
        }
    }
}
