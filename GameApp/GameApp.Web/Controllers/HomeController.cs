using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using GameApp.Web.Models;
using GameApp.Web.Models.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GameApp.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly IGameService gameService;
        public HomeController(IGameService gameService)
        {
            this.gameService = gameService;
        }
        public async Task<IActionResult> Index(int page)
        {
            var model = new HomeIndexViewModel
            {
                PopularGames = await gameService.GetPopularGames(),
                TopRankedGames = await gameService.GetTopRankedGames(),
                UpcomingGames = await gameService.GetUpcomingGames()
            };
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}