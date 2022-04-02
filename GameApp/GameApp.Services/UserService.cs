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
            if (user==null)
            {
                return false;
            }
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
                Games = user.Games.Where(ug=>ug.IsRefunded==false)
                .Select(ug => new GameInfoHelperModel
                {
                    Score = ug.Game.Reviews.Sum(r=>r.Score) / (ug.Game.Reviews.Count()>0? ug.Game.Reviews.Count():1),
                    ImgUrl = ug.Game.ImageUrl,
                    Name = ug.Game.Name

                })
            };

            return model;
        
        
            
        }

        public async Task<UserPhoneAndEmailServiceModel> GetUserPhoneAndEmail(string userId)
        {
            var user =await userManager.FindByIdAsync(userId);

            if (user==null )
            {
                throw new ArgumentException();
            }
            return new UserPhoneAndEmailServiceModel 
            {
                Email=user.Email,
                PhoneNumber=user.PhoneNumber
            };
        }

        public async Task<IEnumerable<UsersListingModel>> GetUsersByName(string username,string userId,int page)
        {
            var a =  this.users
                .All()
                .Include(u => u.Friends)
                .Where(u => u.UserName.ToLower().Contains(username.ToLower())).ToList();
            return await this.users
                .All()
                .Include(u=>u.Friends)
                .Where(u => u.UserName.ToLower().Contains(username.ToLower()))
                .Skip(page*5)
                .Take(5)
                .Select(u=> new UsersListingModel
                {
                    ImgUrl=u.ImgURL ?? "User.png",
                    Games=u.Games.Count(),
                    Username=u.UserName,
                    Description=u.Description,
                    IsFriend=u.Friends.Any(f=>f.FriendUserId==userId ||f.MainUserId==userId)
                }).ToListAsync();
        }

        public async Task<UserSettingsInfoServiceModel> GetUserSettingsInfo(string userId)
        {
            var user = await users
                .All()
                .Include(u=>u.Cards)
                .Include(u => u.Games)
                .ThenInclude(g=>g.Game)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user==null)
            {
                throw new ArgumentException();
            }
            return new UserSettingsInfoServiceModel 
            {
                Description=user.Description,
                Email=user.Email,
                PhoneNumber=user.PhoneNumber,
                Username=user.UserName,
                Games=user.Games.Select(g=>new GameInfoHelperModel
                {
                    ImgUrl=g.Game.ImageUrl,
                    Name=g.Game.Name

                }),
                Cards=user.Cards.Select(c=>new AllCardsServiceListingModel
                {
                    CardNumber=c.CardNumber,
                    CardType=c.CardType,
                    FirstName=c.FirstName,
                    Id=c.Id,
                    LastName=c.LastName
                })
            };
        }

        public async Task<bool> SetEmailAndPhone(string phone, string email,string userId)
        {
            var user =await userManager.FindByIdAsync(userId);
            if (user==null)
            {
                return false;
            }
            user.PhoneNumber = phone;
            user.Email = email;
            users.Update(user);
            await users.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetUsersToCard(Card card, string userId)
        {
            var user=await userManager.FindByIdAsync(userId);
            if (user==null)
            {
                return false;
            }
            card.User = user;
            return true;
        }

        public async Task<bool> SetUsersToComment(Comment comment, string userId)
        {
            var user= await userManager.FindByIdAsync(userId);
            if (user==null)
            {
                return false;
            }
            comment.User = user;
            return true;
        }

        public async Task<bool> SetUsersToFriend(Friend friend, string userId, string friendName)
        {
            var main=await userManager.FindByIdAsync(userId);
            var newFriend= await userManager.FindByNameAsync(friendName);
            if (main==null||newFriend==null)
            {
                return false;
            }
            friend.MainUser = main;
            friend.FriendUser = newFriend;
            return true;
        }

        public async Task<bool> SetUsersToReceipt(Receipt receipt, string userId)
        {
            var user=await userManager.FindByIdAsync(userId);
            if (user==null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> SetUsersToReview(Review review, string userId)
        {
            var user= await userManager.FindByIdAsync(userId);
            if (user==null)
            {
                return false;
            }
            review.User = user;
            return true;

        }
    }
}
