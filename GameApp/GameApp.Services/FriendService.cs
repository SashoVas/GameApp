using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services
{
    public class FriendService : IFriendService
    {
        private readonly IRepository<Friend> friends;

        public FriendService(IRepository<Friend> friends)
        {
            this.friends = friends;
        }

        public async Task<IEnumerable<string>> GetFriends(string userId)
        {
            return await friends.All().Where(f => f.MainUserId == userId && f.Status=="Friend").Select(f => f.FriendUser.UserName).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetRequests(string userId)
        {
            return await friends.All().Where(f => f.MainUserId == userId && f.Status == "Reques").Select(f => f.FriendUser.UserName).ToListAsync();
        }
    }
}
