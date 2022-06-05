using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using GameApp.Services.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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

        public async Task<bool> Deleate(string gameName)
        {
            var game =await games
                .All()
                .Where(g=>g.Name==gameName)
                .Select(g=>new Game 
                {
                    Id=g.Id 
                })
                .FirstOrDefaultAsync();
            games.Delete(game);
            await games.SaveChangesAsync();
            return true;
        }

        public async Task<bool> GameExist(int id)
            => await games.All().AnyAsync(g => g.Id == id);

        public async Task<bool> GameExistByName(string name)
        => await games.All().AnyAsync(g => g.Name == name);

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
                model = model.Where(g => g.Users.Any(gu => gu.User.UserName == username && gu.IsRefunded==false));
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
            var model = await games.All()
                .Where(g => g.Name == name)
                .Select(g=>new GameServiceListingModel
                {
                    Id = g.Id,
                    Description = g.Description,
                    Name = g.Name,
                    Price = g.Price,
                    ImgUrl = g.ImageUrl,
                    Genres = g.Genres.Select(gg => gg.Genre.Name),
                    Users = g.Users.Count(),
                    GameRating = g.Reviews.Sum(r => r.Score) / (g.Reviews.Count() > 0 ? g.Reviews.Count() : 1),
                    ReleaseDate = g.ReleaseDate,
                    Video = g.Video,
                    Reviews=g.Reviews,
                    ReceiptsCount=g.Users.OrderBy(u=>u.Date).LastOrDefault(u=>u.UserId==userId).Receipts.Count()
                }).FirstOrDefaultAsync();

            if (model==null)
            {
                return null;
            }
            model.Rank = games
                .All()
                .Where(g => g.Reviews.Sum(r => r.Score) / (g.Reviews.Count() > 0 ? g.Reviews.Count() : 1) > model.GameRating)
                .Count()+1;

            //this game rank in last 30 days
            var dateDeadline = DateTime.UtcNow.AddDays(-30);
            var myPopularityRating = model.Reviews.Where(r => r.ReviewDate > dateDeadline).Sum(r => r.Score) / (model.Reviews.Where(r => r.ReviewDate > dateDeadline).Count()>0? model.Reviews.Where(r => r.ReviewDate > dateDeadline).Count():1);
            model.Popularity = games
            .All()
            .Select(g=>g.Reviews.Where(r => r.ReviewDate > dateDeadline).Sum(r => r.Score) / (g.Reviews.Where(r => r.ReviewDate > dateDeadline).Count() > 0 ? g.Reviews.Where(r => r.ReviewDate > dateDeadline).Count() : 1))
            .Where(s=>s> myPopularityRating)
            .Count()+1;

            if (model.ReceiptsCount!=0)
            {
                model.HaveGame = model.ReceiptsCount % 2==1;
                var userReview = model.Reviews.SingleOrDefault(r => r.UserId == userId);
                if (userReview!=null)
                {
                    model.UserRating = userReview.Score;
                }
            }

            return model;
        }

        public async Task<PopularGamesServiceListingModel[]> GetPopularGames() 
            => await games.All()
                .OrderByDescending(g => g.Reviews.Where(r => r.ReviewDate > DateTime.UtcNow.AddDays(-30)).Sum(r => r.Score)
                / (g.Reviews.Where(r => r.ReviewDate > DateTime.UtcNow.AddDays(-30)).Count() > 0 ? g.Reviews.Where(r => r.ReviewDate > DateTime.UtcNow.AddDays(-30)).Count() : 1))
                .Take(15)
                .Select(g => new PopularGamesServiceListingModel
                {
                    ImgUrl = g.ImageUrl,
                    Name = g.Name
                }).ToArrayAsync();

        public async Task<PopularGamesServiceListingModel[]> GetTopRankedGames() 
            => await games.All()
                .OrderByDescending(g => g.Reviews.Sum(r => r.Score) / (g.Reviews.Count() > 0 ? g.Reviews.Count() : 1))
                .Take(15)
                .Select(g => new PopularGamesServiceListingModel
                {
                    ImgUrl = g.ImageUrl,
                    Name = g.Name
                }).ToArrayAsync();

        public async Task<PopularGamesServiceListingModel[]> GetUpcomingGames() 
            => await games
                .All()
                .Where(g => g.ReleaseDate > DateTime.UtcNow).Select(g => new PopularGamesServiceListingModel
                {
                    ImgUrl = g.ImageUrl,
                    Name = g.Name
                })
                .Take(5)
                .ToArrayAsync();

        public async Task<bool> IsUpcoming(int gameId) 
            => await games
                .All()
                .Where(g => g.Id == gameId)
                .Select(g => g.ReleaseDate > DateTime.Now)
                .FirstOrDefaultAsync();

        public async Task<bool> SetGameByName(Review review, string gameName)
        {
            var game=await games.All()
                .Where(g => g.Name == gameName)
                .Select(g=>g.Id )
                .FirstOrDefaultAsync();
            if (game==null)
            {
                return false;
            }
            review.GameId = game;
            return true;
        }
    }
}
