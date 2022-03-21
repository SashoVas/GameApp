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
        private readonly IGenreService genreService;
        private readonly IRepository<Game> games;
        private readonly IHostingEnvironment environment;

        public GameService(IRepository<Game> games, IGenreService genreService,
             IHostingEnvironment environment)
        {
            this.games = games;
            this.environment = environment;
            this.genreService = genreService;
        }

        public async Task<bool> AddShoppingCartItem(ShoppingCart shoppingCart, int gameId )
        {
            var game = await games.All().SingleOrDefaultAsync(g => g.Id == gameId);
            if (game==null)
            {
                return false;
            }
            var success=await shoppingCart.AddToCart(game);
            return success;
        }

        public async Task<int> Create(string name, decimal price, string description,DateTime date, IEnumerable<string> newGenres, IFormFile image,string video)
        {
            var game = new Game {
                Name = name,
                Price = price,
                Description = description,
                ReleaseDate=date
            };
            if (video!=null)
            {
                game.Video = video.Split("?v=")[1];
            }

            await genreService.SetGenreToGameByName(game, newGenres);
            
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

        public async Task<IEnumerable<AllGamesServiceListingModel>> GetAll(int page, string gameName, string genre, string username)
        {
            var model = games
                .All()
                .OrderByDescending(g => g.Reviews.Sum(r => r.Score) / (g.Reviews.Count() > 0 ? g.Reviews.Count() : 1))
                .Where(g => g.Name
                .ToLower()
                .Contains(gameName));
                

            if (genre!=null)
            {
                model = model.Where(g => g.Genres.Any(ge => ge.Genre.Name == genre));
            }
            if (username!=null)
            {
                model = model.Where(g => g.Users.Any(gu => gu.User.UserName == username));
            }
            
            return await model
                .Skip(page*10)
                .Take(10)
                .Select(g => new AllGamesServiceListingModel
                {
                    Name = g.Name,
                    Price = g.Price,
                    Genres = g.Genres.Select(gg => gg.Genre.Name).ToList(),
                    ImgUrl = g.ImageUrl,
                    Score= g.Reviews.Sum(r => r.Score) / (g.Reviews.Count() > 0 ? g.Reviews.Count() : 1)
                }).ToListAsync();
                
        }

        

        public async Task< GameServiceListingModel> GetGame(string name,string userId)
        {
            var game =await games
                .All()
                .Include(g=>g.Reviews)
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
                GameRating=game.Reviews.Sum(r => r.Score) / (game.Reviews.Count() > 0 ? game.Reviews.Count() : 1),
                ReleaseDate=game.ReleaseDate,
                Video=game.Video
               
            };
            model.Rank = games
                .All()
                .Where(g => g.Reviews.Sum(r => r.Score) / (g.Reviews.Count() > 0 ? g.Reviews.Count() : 1) > model.GameRating)
                .Count()+1;

            //this game rank in last 30 days
            var dateDeadline = DateTime.UtcNow.AddDays(-30);
            var myPopularityRating = game.Reviews.Where(r => r.ReviewDate > dateDeadline).Sum(r => r.Score) / (game.Reviews.Where(r => r.ReviewDate > dateDeadline).Count()>0? game.Reviews.Where(r => r.ReviewDate > dateDeadline).Count():1);
            model.Popularity = games
            .All()
            .Select(g=>g.Reviews.Where(r => r.ReviewDate > dateDeadline).Sum(r => r.Score) / (g.Reviews.Where(r => r.ReviewDate > dateDeadline).Count() > 0 ? g.Reviews.Where(r => r.ReviewDate > dateDeadline).Count() : 1))
            .Where(s=>s> myPopularityRating)
            .Count()+1;


            
            var usergame = game.Users.SingleOrDefault(g=>g.UserId==userId);
            if (usergame!=null)
            {
                
                model.HaveGame = true;
                var userReview = game.Reviews.SingleOrDefault(r => r.UserId == userId);
                if (userReview!=null)
                {
                    model.UserRating = userReview.Score;
                }
            }

            return model;
        }

        public async Task<PopularGamesServiceListingModel[]> GetPopularGames()
        {
            return await games.All()
                .OrderByDescending(g => g.Reviews.Where(r => r.ReviewDate > DateTime.UtcNow.AddDays(-30)).Sum(r => r.Score) 
                / (g.Reviews.Where(r => r.ReviewDate > DateTime.UtcNow.AddDays(-30)).Count() > 0 ? g.Reviews.Where(r => r.ReviewDate > DateTime.UtcNow.AddDays(-30)).Count() : 1))
                .Take(15)
                .Select(g=>new PopularGamesServiceListingModel 
                { 
                    ImgUrl=g.ImageUrl,
                    Name=g.Name
                }).ToArrayAsync();
        }

        public async Task<PopularGamesServiceListingModel[]> GetTopRankedGames()
        {
            return await games.All()
                .OrderByDescending(g=> g.Reviews.Sum(r => r.Score) / (g.Reviews.Count() > 0 ? g.Reviews.Count() : 1))
                .Take(15)
                .Select(g=>new PopularGamesServiceListingModel 
                { 
                    ImgUrl=g.ImageUrl,
                    Name=g.Name
                }).ToArrayAsync();
        }

        public async Task<PopularGamesServiceListingModel[]> GetUpcomingGames()
        {
            return await games
                .All()
                .Where(g => g.ReleaseDate > DateTime.UtcNow).Select(g=> new PopularGamesServiceListingModel
                {
                    ImgUrl=g.ImageUrl,
                    Name=g.Name
                }) 
                .Take(5)
                .ToArrayAsync();
        }

        public async Task<bool> IsUpcoming(int gameId)
        {
            var game= await games
                .All()
                .FirstOrDefaultAsync(g => g.Id == gameId);
            
            return game.ReleaseDate > DateTime.Now;
        }

        public async Task<bool> RemoveShoppingCartItem(ShoppingCart shoppingCart, int gameId)
        {
            var game = await games.All().SingleOrDefaultAsync(g => g.Id == gameId);
            if (game==null)
            {
                return false;
            }
            var success=await shoppingCart.RemoveFromCart(game);
            return success;
        }

        public async Task<bool> SetGameById(Comment comment,int gameId)
        {
            var game=await games.All().SingleOrDefaultAsync(g => g.Id==gameId);
            if (game==null)
            {
                return false;
            }
            comment.Game = game;
            return true;
        }

        public async Task<bool> SetGameByName(Review review, string gameName)
        {
            var game=await games.All().SingleOrDefaultAsync(g => g.Name == gameName);
            if (game==null)
            {
                return false;
            }
            review.Game = game;
            return true;
        }


    }
}
