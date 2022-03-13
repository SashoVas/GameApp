﻿using GameApp.Data.Models;
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

        public async Task<bool> ChangeStatus(string userId, string username, FriendStatus status, FriendStatus only)
        {
            var friend =await friends
                .All()
                .SingleOrDefaultAsync(f=>f.MainUserId==userId
                &&f.FriendUser.UserName==username||f.FriendUserId==userId && f.MainUser.UserName==username);
            if (friend==null||friend.Status!=only)
            {
                return false;
            }
            friend.Status = status;
            friends.Update(friend);
            await friends.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<string>> GetFriends(string userId)
        {
            return await friends.All().Where(f => f.MainUserId == userId && f.Status==FriendStatus.Friend || f.FriendUserId == userId && f.Status == FriendStatus.Friend).Select(f => f.MainUserId == userId ? f.FriendUser.UserName : f.MainUser.UserName).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetRequests(string userId)
        {
            return await friends.All().Where(f => f.FriendUserId == userId && f.Status == FriendStatus.Request).Select(f => f.MainUser.UserName).ToListAsync();
        }

        public async Task<bool> SendFriendRequest(string userId,string username)
        {
            var friend = await friends
                .All()
                .Include(f=>f.MainUser)
                .Include(f=>f.FriendUser)
                .SingleOrDefaultAsync(f => f.MainUserId == userId 
                && f.FriendUser.UserName == username || f.FriendUserId == userId && f.MainUser.UserName == username);
            if (friend==null)
            {
                friend = new Friend {Status= FriendStatus.Request };
                var hasFriend=await userService.SetUsersToFriend(friend,userId,username);
                if (!hasFriend)
                {
                    return false;
                }
                await friends.AddAsync(friend);
            }
            else
            {
                friend.Status = FriendStatus.Request;
                if (friend.FriendUserId==userId)
                {
                    var mainfriend = friend.MainUser;
                    friend.MainUser = friend.FriendUser;
                    friend.FriendUser = mainfriend;
                }
                friends.Update(friend);
            }
            await friends.SaveChangesAsync();
            return true;
        }
    }
}
