using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using GameApp.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace GameApp.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly IRepository<UserGame> userGames;
        private readonly IHostingEnvironment environment;

        public UserService(UserManager<User> userManager, IRepository<UserGame> userGames, IHostingEnvironment environment)
        {
            this.userManager = userManager;
            this.userGames = userGames;
            this.environment = environment;
        }

        public async Task<bool> ChangeImage(IFormFile image,string userId)
        {
            var user  =await userManager.FindByIdAsync(userId);
           
            var imgId = Guid.NewGuid().ToString();
            using (var file = File.OpenWrite(environment.WebRootPath + "/Files/" + imgId + ".png"))
            {
                image.CopyTo(file);
                
            }
            user.ImgURL = imgId + ".png";
            await userManager.UpdateAsync(user);
            
            return true;
        }

        public async Task<UserInfoServiceModel> GetUserInfo(string username)
        {
            var user= await userManager.FindByNameAsync(username);
            return new UserInfoServiceModel {
                UserName = user.UserName,
                ProfilePic = user.ImgURL,
                Games =await userGames
                .All()
                .Where(ug => ug.UserId == user.Id)
                .Select(ug => ug.Game.Name)
                .ToListAsync()
            };
        }
    }
}
