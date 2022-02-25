using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services;
using GameApp.Tests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            var newUser1 = new User 
            { 
                Id="NewUser1",
                UserName="NewUser1"
            };

            var newUser2 = new User
            {
                Id = "NewUser2",
                UserName = "NewUser2"
            };

            await context.Users.AddAsync(newUser1);
            await context.Users.AddAsync(newUser2);
            await context.SaveChangesAsync();

            var store = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            userManager.Setup(um => um.FindByIdAsync("NewUser1")).Returns(async()=>newUser1);
            userManager.Setup(um => um.FindByNameAsync("NewUser2")).Returns(async()=>newUser2);

            var userService = new UserService(userManager.Object,null,null);


            var friendService = new FriendService(repo,userService);

            Assert.True( await friendService.SendFriendRequest("NewUser1", "NewUser2"));

            var result = repo
                .All()
                .Last();

            var actualData = new Friend
            {
                FriendUser=newUser2,
                MainUser=newUser1,
                FriendUserId=newUser2.Id,
                MainUserId=newUser1.Id,
                Status=FriendStatus.Request
            };

            Assert.Equal(result.FriendUserId,actualData.FriendUserId);
            Assert.Equal(result.MainUserId, actualData.MainUserId);
            Assert.Equal(result.Status, actualData.Status);
            Assert.Equal(result.FriendUser.UserName, actualData.FriendUser.UserName);
            Assert.Equal(result.MainUser.UserName, actualData.MainUser.UserName);

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


            Assert.Equal(result.Status, FriendStatus.Request);
            Assert.Equal(result.FriendUser.UserName, "MainUserRejected");
            Assert.Equal(result.FriendUser.Id, "MainUserRejected");
            Assert.Equal(result.MainUser.Id, "FriendUserRejected");
            Assert.Equal(result.MainUser.UserName, "FriendUserRejected");

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

    }
}
