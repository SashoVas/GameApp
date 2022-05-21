using GameApp.Services.Contracts;
using GameApp.Web.Models.Genre;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.Web.Controllers
{
    public class GenreController : Controller
    {
        private readonly IGenreService genreService;
        public GenreController(IGenreService genreService)
        {
            this.genreService = genreService;
        }
        public IActionResult Create()
        {
            return View();
        }
        //[Authorize(Roles ="admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateGenreInputModel input)
        {
            if(!this.ModelState.IsValid)
            {
                return View(input);
            }
            if (!await genreService.Create(input.Name))
            {
                return this.BadRequest();
            }
            return this.Redirect("/");

        }
    }
}
