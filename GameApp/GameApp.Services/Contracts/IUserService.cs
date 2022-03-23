using GameApp.Data.Models;
using GameApp.Services.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services.Contracts
{
    public interface IUserService
    {
        Task<UserInfoServiceModel> GetUserInfo(string username);
        Task<UserSettingsInfoServiceModel> GetUserSettingsInfo(string userId);
        Task<bool> ChangeImage(IFormFile image, string userId);
        Task<bool> EditDescription(string description, string userId);
        Task<bool> SetUsersToFriend(Friend friend,string userId, string friendName);
        Task<bool> SetUsersToCard(Card card, string userId);
        Task<bool> SetUsersToReceipt(Receipt receipt, string userId);
        Task<bool> SetUsersToComment(Comment comment, string userId);
        Task<bool> SetUsersToReview(Review review, string userId);
        Task<IEnumerable<UsersListingModel>> GetUsersByName(string username, string userId,int page);
    }
}
