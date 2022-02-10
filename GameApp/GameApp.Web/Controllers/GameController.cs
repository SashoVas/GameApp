using GameApp.Services.Contracts;
using GameApp.Web.Models;
using GameApp.Web.Models.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameApp.Web.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameService gamesService;
        private readonly ICartService cartService;
        private readonly IReviewService reviewService;
        public GameController(IGameService gamesService, ICartService cartService, IReviewService reviewService)
        {
            this.gamesService = gamesService;
            this.cartService = cartService;
            this.reviewService = reviewService;
        }
        [Route("Game")]
        public async Task<IActionResult> Game(string title)
        {
            var game=await gamesService
                .GetGame(title,this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (game==null)
            {
                return this.NotFound();
            }

            return this.View(new GameViewModel
            {
                Id = game.Id,
                Description = game.Description,
                Price = game.Price,
                Name = game.Name,
                ImageUrl = game.ImgUrl,
                Genres = game.Genres,
                UserRating = game.UserRating,
                Users = game.Users,
                HaveGame = game.HaveGame,
                Rank = game.Rank,
                Popularity = game.Popularity,
                ReleaseDate = game.ReleaseDate,
                Score = game.GameRating
            }); 

        }
        [Route("Game/AllGames/{page?}")]
        public async Task<IActionResult> AllGames(int page,string gameName,string genre,string username)
        {
            if (gameName==null)
            {
                gameName = "";
            }
            var model =new AllGamesViewModel { Games =await gamesService.GetAll(page,gameName.ToLower(),genre,username) };
            return this.View(model);
        }
        //[Authorize(Roles ="admin")]
        [HttpGet()]
        public IActionResult Create() 
        {
            
            return View();
        }
        //[Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateGameInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }
            int id=await gamesService.Create(model.Name,model.Price,model.Description,model.Genres,model.Image);
            return this.RedirectToAction(nameof(AllGames));
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult>Buy()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await gamesService.BuyItems(userId);

            return this.Redirect("/");
        }

        [HttpPost]
        public async  Task<IActionResult> Rate(GameRateInputModel rate)
        {
            await reviewService.Rate(rate.GameName,rate.Points,this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return this.Redirect("~/Game?title="+rate.GameName);
        }
    }
}
