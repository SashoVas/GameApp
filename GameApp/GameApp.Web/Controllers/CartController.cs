using GameApp.Services.Contracts;
using GameApp.Web.Models.Cart;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GameApp.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService cartService;
        private readonly IGameService gameService;
        public CartController(ICartService cartService, IGameService gameService)
        {
            this.cartService = cartService;
            this.gameService = gameService;
        }
        [Route("/Cart")]
        public async Task<IActionResult> Cart()
        {
            var model = new AllItemsInCartViewModel {
            Items= await cartService.GetAllItems()
            };
            return View(model);
        }
        [HttpPost]
        public async Task< IActionResult> AddToCart([Required]int gameId)
        {
            var isUpcoming=await gameService.IsUpcoming(gameId);
            if (isUpcoming)
            {
                return this.BadRequest();
            }
            bool accepted=await cartService.AddToCart(gameId);
            if (!accepted)
            {
                return this.BadRequest();
            }
            return this.RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        public async Task<IActionResult>Remove([Required] int id)
        {
            var success=await cartService.RemoveFromCart(id);
            if (!success)
            {
                return this.BadRequest();
            }
            return this.RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        public async Task<IActionResult>Clear()
        {
            await cartService.Clear();
            return this.Redirect("/");
        }
    }
}
