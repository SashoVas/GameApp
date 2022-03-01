using Microsoft.AspNetCore.Mvc;

namespace GameApp.Web.Controllers
{
    public class CardController : Controller
    {
        public IActionResult SetCard()
        {
            return View();
        }
    }
}
