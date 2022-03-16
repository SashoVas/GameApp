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
        [Route("Profile/MyUser/ProfileInfo/{name}")]
        public async Task<IActionResult> ProfileInfo(string name)
        {
            var userInfo =await userService.GetUserInfo(name);
            if (userInfo==null)
            {
                return this.NotFound();
            }
            var model = new ProfileInfoViewModel 
            {
                UserName=userInfo.UserName,
                ProfilePic=userInfo.ProfilePic ?? "User.png",
                Games=userInfo.Games,
                Description=userInfo.Description,
                ScoreCounts=userInfo.ScoreCount,
                MeanScore=userInfo.MeanScore,
                Reviews=userInfo.ReviewsCount
            };
            return this.View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeDescription(string description) 
        {
            await userService.EditDescription( description,this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return this.Redirect("/Profile/MyUser/ProfileInfo/"+this.User.Identity.Name);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProfilePic([Required]IFormFile file)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await userService.ChangeImage(file,userId);
            return this.Redirect("/Profile/MyUser/ProfileInfo/" + this.User.Identity.Name);
        }
       
        public async Task<IActionResult> SearchUser([Required]string username,int page)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }
            var model = new UsersViewModel
            {
                Users =await userService.GetUsersByName(username, this.User.FindFirstValue(ClaimTypes.NameIdentifier),page),
                Page=page,
                Username=username

            };
            return this.View(model);
        }
    }
}
