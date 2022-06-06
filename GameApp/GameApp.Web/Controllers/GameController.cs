using GameApp.Services.Contracts;
using GameApp.Web.Models.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace GameApp.Web.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameService gamesService;
        private readonly ICartService cartService;
        private readonly IReviewService reviewService;
        private readonly ICardService cardService;
        public GameController(IGameService gamesService, ICartService cartService, IReviewService reviewService, ICardService cardService)
        {
            this.gamesService = gamesService;
            this.cartService = cartService;
            this.reviewService = reviewService;
            this.cardService = cardService;
        }
        [Route("Game")]
        public async Task<IActionResult> Game([Required]string title)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }
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
                Score = game.GameRating,
                SimilarGames=await gamesService.GetPopularGames(),
                Video=game.Video
            }); 

        }
        [Route("Game/AllGames/{page?}")]
        public async Task<IActionResult> AllGames(SearchGameInfoModel info)
        {
            if (info.GameName==null)
            {
                info.GameName = "";
            }
            var model =new AllGamesViewModel {
                Games =await gamesService.GetAll(info.Page, info.GameName.ToLower(),info.Genre,info.Username),
                SearchInfo=info
            };
            return this.View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet()]
        public IActionResult Create() 
            => View();
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateGameInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }
            int id=await gamesService.Create(model.Name,model.Price,model.Description,model.Date,model.Genres,model.Image,model.VideoUrl);
            return this.RedirectToAction(nameof(AllGames));
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult>Buy([Required]string cardId)
        {
            if (!ModelState.IsValid||!await cardService.CardExist(cardId))
            {
                return this.BadRequest();
            }
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var success=await cartService.BuyItems(userId, cardId);
            if (!success)
            {
                this.BadRequest();
            }
            HttpContext.Items["userId"] = userId;
            return this.Redirect("/");
        }

        [HttpPost]
        public async  Task<IActionResult> Rate(GameRateInputModel rate)
        {
            if (!ModelState.IsValid ||!await gamesService.GameExistByName(rate.GameName))
            {
                return this.Redirect("~/Game?title=" + rate.GameName);
            }
            var success=await reviewService.Rate(rate.GameName,rate.Points,this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!success)
            {
                return this.BadRequest();
            }
            return this.Redirect("~/Game?title="+rate.GameName);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult>Deleate([Required]string gameName)
        {
            if (!ModelState.IsValid||!await gamesService.GameExistByName(gameName))
            {
                return this.BadRequest();
            }
            await gamesService.Deleate(gameName);
            return this.Redirect("/");
        }
    }
}
