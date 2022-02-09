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
        Task<bool> ChangeImage(IFormFile image, string userId);
        Task<bool> EditDescription(string description, string userId);
    }
}
