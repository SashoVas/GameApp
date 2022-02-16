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
        private readonly IUserService userService;
        public FriendService(IRepository<Friend> friends, IUserService userService)
        {
            this.friends = friends;
            this.userService = userService;
        }

        public async Task<bool> ChangeStatus(string userId, string username, string status)
        {
            var friend =await friends
                .All()
                .SingleOrDefaultAsync(f=>f.MainUserId==userId
                &&f.FriendUser.UserName==username);
            friend.Status = status;
            friends.Update(friend);
            await friends.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<string>> GetFriends(string userId)
        {
            return await friends.All().Where(f => f.MainUserId == userId && f.Status=="Friend").Select(f => f.FriendUser.UserName).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetRequests(string userId)
        {
            return await friends.All().Where(f => f.MainUserId == userId && f.Status == "Request").Select(f => f.FriendUser.UserName).ToListAsync();
        }

        public async Task<bool> SendFriendRequest(string userId,string username)
        {
            var friend = await friends
                .All()
                .SingleOrDefaultAsync(f => f.MainUserId == userId 
                && f.FriendUser.UserName == username);
            if (friend==null)
            {
                friend = new Friend {Status= "Request" };
                await userService.SetUsersToFriend(friend,userId,username);
                await friends.AddAsync(friend);
            }
            else
            {
                friend.Status = "Request";
                friends.Update(friend);
            }
            await friends.SaveChangesAsync();
            return true;
        }
    }
}
