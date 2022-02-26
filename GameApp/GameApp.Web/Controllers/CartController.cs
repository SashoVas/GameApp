using GameApp.Services.Contracts;
using GameApp.Web.Models.Cart;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GameApp.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService cartService;
        public CartController(ICartService cartService)
        {
            this.cartService = cartService;
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
        public async Task< IActionResult> AddToCart([Required]int id)
        {
            bool accepted=await cartService.AddToCart(id);

            return this.RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        public async Task<IActionResult>Remove([Required] int id)
        {
            await cartService.RemoveFromCart(id);
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
