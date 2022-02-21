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
    public class UserServiceTests
    {
        private List<User> GetDummyData()
        {
            var users = new List<User>();
            for (int i = 0; i < 16; i++)
            {
                users.Add(new User 
                { 
                    UserName="UserName"+i.ToString(),
                    Id=i.ToString(),
                    Description="nt"
                });
            }
            return users;
        }
        private async Task SeedData(GameAppDbContext context)
        {
            context.Users.AddRange(GetDummyData());
            await context.SaveChangesAsync();

        }
        [Theory]
        [InlineData("UserName2")]
        [InlineData("UserName12")]
        public async Task TestUserInfoShouldReturnUserInfo(string userName)
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var store = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            
            var userService = new UserService(userManagerMock.Object, null,new Repository<User>(context));

            var result =await userService.GetUserInfo(userName);
            var actualData = GetDummyData().SingleOrDefault(u => u.UserName == userName);
            Assert.Equal(result.UserName, actualData.UserName);
            Assert.Equal(result.Description, actualData.Description);
            Assert.Equal(result.ProfilePic, actualData.ImgURL);
            

        }
        [Fact]
        public async Task TestUserInfoWithWrongValueShouldReturnNull()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var userService = new UserService(null, null, new Repository<User>(context));

            Assert.Null(await userService.GetUserInfo("asdf"));
            Assert.Null(await userService.GetUserInfo(""));
            Assert.Null(await userService.GetUserInfo(null));
        }
        [Theory]
        [InlineData("3")]
        [InlineData("6")]
        public async Task TestEditDescriptionShouldEditDescription(string userId)
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var store = new Mock<IUserStore<User>>();
            
            var userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            var actualData = GetDummyData();
            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .Returns(async()=> actualData.SingleOrDefault(u=>u.Id== userId));
            var userService = new UserService(userManagerMock.Object, null, new Repository<User>(context));
            Assert.True(await userService.EditDescription("smt",userId));
            var changedDescription = actualData.SingleOrDefault(u => u.Id == userId);
            Assert.Equal(changedDescription.Description,"smt");
        }
    }
}
