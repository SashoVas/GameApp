using GameApp.Services.Contracts;
using GameApp.Web.Areas.Profile.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace GameApp.Web.Areas.Profile.Controllers
{
    [Area("Profile")]
    public class MyUserController : Controller
    {
        private readonly IUserService userService;
        private readonly IUserGameService userGameService;
        public MyUserController(IUserService userService, IUserGameService userGameService)
        {
            this.userService = userService;
            this.userGameService = userGameService;
        }
        [Route("Profile/MyUser/ProfileInfo/{name}")]
        public async Task<IActionResult> ProfileInfo([Required]string name)
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
        [Authorize]
        public async Task<IActionResult> ChangeDescription([Required][MaxLength(500)]string description) 
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
        [Authorize]
        public async Task<IActionResult>Settings()
        {
            var userInfo = await userService.GetUserSettingsInfo(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userInfo == null)
            {
                return this.NotFound();
            }
            var model = new UserSettingsViewModel
            {
                Username = userInfo.Username,
                Description = userInfo.Description,
                PhoneNumber=userInfo.PhoneNumber,
                Email=userInfo.Email,
                Cards=userInfo.Cards,
                Games=userInfo.Games
            };
            return this.View(model);
        }
        [Authorize]
        public async Task<IActionResult>ChangePhoneAndEmail()
        {
            var phoneAndEmail =await userService.GetUserPhoneAndEmail(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (phoneAndEmail==null)
            {
                return this.BadRequest();
            }
            return this.View(new PhoneAndEmailInputModel 
            {
                Email=phoneAndEmail.Email,
                PhoneNumber=phoneAndEmail.PhoneNumber
            });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult>ChangePhoneAndEmail(PhoneAndEmailInputModel input)
        {
           
            var succes =await this.userService.SetEmailAndPhone(input.PhoneNumber,input.Email,this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return this.Redirect(nameof(Settings));
        }
        [Authorize]
        public async Task<IActionResult>RefundGame()
        {
            var games =await userGameService.GetGameForRefund(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return this.View(new RefundGameViewModel 
            { 
                Games=games
            });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult>RefundGame([Required]int gameId)
        {
            if (!ModelState.IsValid)
            {
                return this.View();
            }
            var success =await this.userGameService.RefundGame(gameId,this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!success)
            {
                return this.BadRequest();
            }
            return this.Redirect(nameof(Settings));
        }
    }
}
