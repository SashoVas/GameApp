using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using GameApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GameApp.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly IGameService gameService;
        private readonly ShoppingCart shoppingCart;
        public HomeController(IGameService gameService, ShoppingCart shoppingCart)
        {
            this.gameService = gameService;
            this.shoppingCart = shoppingCart;
        }
        public async Task<IActionResult> Index(int page)
        {

            return View();
        }
        [Authorize(Roles ="admin")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}