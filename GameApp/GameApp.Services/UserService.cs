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
        private readonly IRepository<User> users;
        private readonly IHostingEnvironment environment;

        public UserService(UserManager<User> userManager, IHostingEnvironment environment, IRepository<User> users)
        {
            this.userManager = userManager;
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
            var user = await users.All()
                .Include(u => u.Reviews)
                .Include(u=>u.Games)
                .ThenInclude(ug=>ug.Game)
                .ThenInclude(g=>g.Reviews)
                .SingleOrDefaultAsync(u => u.UserName == username);
            if (user==null)
            {
                return null;
            }
            user.Reviews.ToList().ForEach(x => arr[(int)x.Score]++);
            var reviews = user.Reviews.Count();
            var model=new UserInfoServiceModel
            {
                UserName = user.UserName,
                ProfilePic = user.ImgURL,
                Description = user.Description,
                ScoreCount = arr,
                MeanScore = user.Reviews.Sum(r => r.Score) / (reviews>0?reviews:1),
                ReviewsCount=reviews,
                Games = user.Games
                .Select(ug => new GameInfoHelperModel
                {
                    Score = ug.Game.Reviews.Sum(r=>r.Score) / (ug.Game.Reviews.Count()>0? ug.Game.Reviews.Count():1),
                    ImgUrl = ug.Game.ImageUrl,
                    Name = ug.Game.Name

                })
            };

            return model;
        
        
            
        }
        public async Task<bool> SetUsersToCard(Card card, string userId)
        {
            card.User =await userManager.FindByIdAsync(userId);
            return true;
        }

        public async Task<bool> SetUsersToFriend(Friend friend, string userId, string friendName)
        {
            friend.MainUser =await userManager.FindByIdAsync(userId);
            friend.FriendUser = await userManager.FindByNameAsync(friendName);
            return true;
        }
    }
}
