using GameApp.Services.Contracts;
using GameApp.Web.Areas.Profile.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace GameApp.Web.Areas.Profile.Controllers
{
    [Area("Profile")]
    public class MyUserController : Controller
    {
        private readonly IUserService userService;
        public MyUserController(IUserService userService)
        {
            this.userService = userService;
        }
        public async Task<IActionResult> ProfileInfo()
        {
            var userInfo =await userService.GetUserInfo(this.User.Identity.Name);
            var model = new ProfileInfoViewModel 
            {
                UserName=userInfo.UserName,
                ProfilePic=userInfo.ProfilePic ?? "User.png",
                Games=userInfo.Games,
            };
            return this.View(model);
        }
        public IActionResult ChangeProfilePic()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProfilePic([Required]IFormFile file)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await userService.ChangeImage(file,userId);
            return this.Redirect(nameof(ProfileInfo));
        }
    }
}
