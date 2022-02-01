using GameApp.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.Web.Infrastructure.ViewComponents
{
    public class SelectGenreViewComponent:ViewComponent
    {
        private readonly IGenreService genreService;
        public SelectGenreViewComponent(IGenreService genreService)
        {
            this.genreService = genreService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model =await genreService.GetAll();
            return View(model);
        }

    }
}
