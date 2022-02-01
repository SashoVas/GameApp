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
        public GameController(IGameService gamesService, ICartService cartService)
        {
            this.gamesService = gamesService;
            this.cartService = cartService;
        }
        public async Task<IActionResult> Library() 
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = new AllGamesViewModel {
            Games=await gamesService.MyGames(userId)
            };
            return View(model);
        }
        [Route("Game")]
        public IActionResult Game(string title)
        {
            var game=gamesService.GetGame(title);
            if (game==null)
            {
                return this.NotFound();
            }

            return this.View(new GameViewModel 
            { 
                Id=game.Id,
                Description=game.Description,
                Price=game.Price,
                Name=game.Name,
                ImageUrl=game.ImgUrl
            });
        }
        [Route("Game/AllGames/{page?}")]
        public IActionResult AllGames(int page,string gameName)
        {
            if (gameName==null)
            {
                gameName = "";
            }
            var model =new AllGamesViewModel { Games = gamesService.GetAll(page,gameName.ToLower()) };
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
            int id=await gamesService.Create(model.Name,model.Price,model.Description,model.Genres);
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
    }
}
