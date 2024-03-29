﻿using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services;
using GameApp.Services.Contracts;
using GameApp.Tests.Infrastructure;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GameApp.Tests.Services
{
    public class FriendServiceTests
    {
        private List<Friend> GetDummyData()
        {
            var friends = new List<Friend>();

            for (int i = 1; i < 16; i++)
            {
                var friend = new Friend
                {
                    Id = i,
                    Status = i % 2 == 0 ? FriendStatus.Friend : FriendStatus.Request,
                    FriendUser= new User 
                    { 
                        Id= "FriendUser"+i.ToString(),
                        UserName="FriendUser"+i.ToString()
                    },
                    MainUser= new User 
                    {
                        Id = "MainUser" + i.ToString(),
                        UserName = "MainUser" + i.ToString()
                    }
                };
                friends.Add(friend);
            }

            return friends;
        }
        private async Task SeedData(GameAppDbContext context)
        {
            context.AddRange(GetDummyData());
            await context.SaveChangesAsync();
        }
        [Fact]
        public async Task TestGetFriendsShouldReturnFriends()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Friend>(context);

            var friendService = new FriendService(repo,null);

            var all = repo.All().ToList();

            var result = (await friendService.GetFriends("MainUser2")).ToList();

            var actualData = repo.All()
                .Where(f => f.MainUserId == "MainUser2" && f.Status == FriendStatus.Friend)
                .ToList();

            for (int i = 0; i < result.Count(); i++)
            {
                Assert.Equal(result[i], actualData[i].FriendUser.UserName);
            }
        }

        [Fact]
        public async Task TestGetFriendsRequestShouldReturnFriendsRequest()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Friend>(context);

            var friendService = new FriendService(repo, null);

            var all = repo.All().ToList();

            var result = (await friendService.GetRequests("FriendUser1")).ToList();

            var actualData = repo.All()
                .Where(f => f.FriendUserId == "FriendUser1" && f.Status == FriendStatus.Request)
                .ToList();

            for (int i = 0; i < result.Count(); i++)
            {
                Assert.Equal(result[i], actualData[i].MainUser.UserName);
            }
        }
        [Fact]
        public async Task TestSendFriendRequestShouldCreateNewFriend()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Friend>(context);
           
            var userService = new Mock<IUserService>();
            userService.Setup(u => u.SetUsersToFriend(It.IsAny<Friend>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(async(Friend friend,string userId,string friendName) => 
                { 
                    friend.MainUserId = userId;
                    friend.FriendUserId = "NewUser2";
                    return true;
                });
            var friendService = new FriendService(repo,userService.Object);

            Assert.True( await friendService.SendFriendRequest("NewUser1", "NewUser2"));

            var result = repo
                .All()
                .Last();

            Assert.Equal("NewUser2", result.FriendUserId);
            Assert.Equal("NewUser1", result.MainUserId);
            Assert.Equal(FriendStatus.Request, result.Status);
        }
        [Theory]
        [InlineData("NewUser1", null)]
        [InlineData("NewUser1", "NoUser")]   
        public async Task TestSendFriendRequestWithImproperDataShouldReturnFalse(string userId,string username)
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Friend>(context);
            var userService = new Mock<IUserService>();
            userService.Setup(us => us.SetUsersToFriend(It.IsAny<Friend>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(async() =>false);
            var friendService = new FriendService(repo, userService.Object);

            Assert.False(await friendService.SendFriendRequest(userId, username));
        }

        [Fact]
        public async Task TestSendFriendRequestShouldChangeOldOne()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Friend>(context);

            var friend = new Friend
            {
                Id = 50,
                Status = FriendStatus.Rejected,
                FriendUser = new User
                {
                    Id = "FriendUserRejected",
                    UserName = "FriendUserRejected"
                },
                MainUser = new User
                {
                    Id = "MainUserRejected",
                    UserName = "MainUserRejected"
                }
            };
            await repo.AddAsync(friend);
            await context.SaveChangesAsync();
            var friendService = new FriendService(repo, null);
            await friendService.SendFriendRequest("FriendUserRejected", "MainUserRejected");

            var result = repo.All().SingleOrDefault(f => f.Id == 50);


            Assert.Equal(FriendStatus.Request, result.Status);
            Assert.Equal("MainUserRejected", result.FriendUser.UserName);
            Assert.Equal("MainUserRejected", result.FriendUser.Id);
            Assert.Equal("FriendUserRejected", result.MainUser.Id);
            Assert.Equal("FriendUserRejected", result.MainUser.UserName);

        }
        [Fact]
        public async Task TestChangeStatusShouldChangeStatus()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Friend>(context);
            var friendService=new FriendService(repo,null);

            await friendService.ChangeStatus("MainUser2","FriendUser2",FriendStatus.Rejected,FriendStatus.Friend);

            var result = repo
                .All()
                .SingleOrDefault(f=>f.MainUserId== "MainUser2"&&f.FriendUserId== "FriendUser2");

            Assert.Equal(result.Status,FriendStatus.Rejected);
        }

        [Theory]
        [InlineData(null, "FriendUser2", FriendStatus.Rejected, FriendStatus.Friend)]
        [InlineData("MainUser2", null, FriendStatus.Rejected, FriendStatus.Friend)]
        [InlineData("MainUser2", "FriendUser2", FriendStatus.Friend, FriendStatus.Rejected)]
        [InlineData("MainUser2", "FriendUser2", FriendStatus.Rejected, FriendStatus.Rejected)]
        [InlineData("MainUser2", "FriendUser2", FriendStatus.Request, FriendStatus.Request)]
        public async Task TestChangeStatusWithImproperDataShouldReturnFalse(string userId, string username,FriendStatus friendStatus1,FriendStatus friendStatus2)
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Friend>(context);
            var friendService = new FriendService(repo, null);

            Assert.False(await friendService.ChangeStatus(userId, username, friendStatus1, friendStatus2));
        }


    }
}
