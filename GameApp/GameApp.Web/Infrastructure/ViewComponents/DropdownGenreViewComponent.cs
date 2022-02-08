using GameApp.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.Web.Infrastructure.ViewComponents
{
    public class DropdownGenreViewComponent: ViewComponent
    {
        private readonly IGenreService genreService;
        public DropdownGenreViewComponent(IGenreService genreService)
        {
            this.genreService = genreService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await genreService.GetAll();
            return View(model);
        }
    }
}
