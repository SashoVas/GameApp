using GameApp.Data.Models;
using GameApp.Services.Models;
using Microsoft.AspNetCore.Http;

namespace GameApp.Services.Contracts
{
    public interface IUserService
    {
        Task<UserInfoServiceModel> GetUserInfo(string username);
        Task<UserPhoneAndEmailServiceModel> GetUserPhoneAndEmail(string userId);
        Task<UserSettingsInfoServiceModel> GetUserSettingsInfo(string userId);
        Task<bool> ChangeImage(IFormFile image, string userId);
        Task<bool> SetEmailAndPhone(string phone,string email, string userId);
        Task<bool> EditDescription(string description, string userId);
        Task<bool> SetUsersToFriend(Friend friend,string userId, string friendName);
        Task<IEnumerable<UsersListingModel>> GetUsersByName(string username, string userId,int page);
    }
}
