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
        private readonly IRepository<User> users;
        private readonly IHostingEnvironment environment;

        public UserService(UserManager<User> userManager, IRepository<UserGame> userGames, IHostingEnvironment environment, IRepository<User> users)
        {
            this.userManager = userManager;
            this.userGames = userGames;
            this.environment = environment;
            this.users = users;
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

        public async Task<bool> EditDescription(string description, string userId)
        {
            var user =await userManager.FindByIdAsync(userId);
            user.Description = description;
            await userManager.UpdateAsync(user);
            return true;
        }

        public async Task<UserInfoServiceModel> GetUserInfo(string username)
        {
            var arr = new int[11];
            var user = await users.All().Include(u => u.Games).ThenInclude(ug => ug.Game).SingleOrDefaultAsync(u => u.UserName == username);
            user.Games.ToList().ForEach(x => arr[(int)x.Rating]++);
            var reviews = user.Games.Where(ug => ug.Rating != 0).Count();
            return new UserInfoServiceModel
            {
                UserName = user.UserName,
                ProfilePic = user.ImgURL,
                Description = user.Description,
                ScoreCount = arr,
                MeanScore = user.Games.Where(ug => ug.Rating != 0).Sum(ug => ug.Rating) / reviews,
                ReviewsCount=reviews,
                Games = user.Games
                .Select(ug => new GameInfoHelperModel
                {
                    Score = ug.Game.RatingSum / (ug.Game.Users.Where(rev => rev.Rating != 0).Count()),
                    ImgUrl = ug.Game.ImageUrl,
                    Name = ug.Game.Name

                })
            };
        
        
            
        }
    }
}
