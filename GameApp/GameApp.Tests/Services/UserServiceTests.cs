using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services;
using GameApp.Tests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
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
                    Description="nt",
                    ImgURL="User.png"
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
        [Theory]
        [InlineData("NotUser")]
        public async Task TestEditDescriptionWithImproperDataReturnFalse(string userId)
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var store = new Mock<IUserStore<User>>();

            var userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            var actualData = GetDummyData();
            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .Returns(async () => actualData.SingleOrDefault(u => u.Id == userId));
            var userService = new UserService(userManagerMock.Object, null, new Repository<User>(context));
            Assert.False(await userService.EditDescription("smt", userId));
        }

        [Fact]
        public async Task TestGetUsersShouldReturnMatchingUsers()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<User>(context);
            var userService = new UserService(null, null,repo );

            var result = (await userService.GetUsersByName("1",null,0)).ToList();

            var actual = repo.All().Where(u => u.UserName.ToLower().Contains("1")).ToList() ;

            for (int i = 0; i < result.Count; i++)
            {
                Assert.Equal(result[i].Username, actual[i].UserName);
                Assert.Equal(result[i].Description, actual[i].Description);
                Assert.Equal(result[i].ImgUrl, actual[i].ImgURL);
            }
        }
        [Fact]
        public async Task TestGetUsersWithImproperDataShouldReturnEmptyArray()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<User>(context);
            var userService = new UserService(null, null, repo);

            var result = (await userService.GetUsersByName("not a name",null,0)).ToList();
            Assert.Empty(result);
        }
        [Fact]
        public async Task TestGetUserSettingsInfoShouldReturnUserInfo()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var store = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            var actualData = GetDummyData().SingleOrDefault(u => u.Id == "1");
            userManagerMock.Setup(u => u.FindByIdAsync("1")).Returns(async()=> actualData);
            var userService = new UserService(userManagerMock.Object, null, new Repository<User>(context));

            var result = await userService.GetUserSettingsInfo("1");
            
            Assert.Equal(result.Username, actualData.UserName);
            Assert.Equal(result.Description, actualData.Description);
            Assert.Equal(result.Email, actualData.Email);
            Assert.Equal(result.PhoneNumber, actualData.PhoneNumber);
        }
        [Fact]
        public async Task TestGetUserSettingsInfoRetturenNull()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var userService = new UserService(null, null, new Repository<User>(context));
            Assert.Null(await userService.GetUserSettingsInfo("-1"));
        }
        [Fact]
        public async Task TestSetEmailAndPhoneShouldChangeThem()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var store = new Mock<IUserStore<User>>();
            var repo = new Repository<User>(context);
            var userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            var actualData = await repo.All().FirstOrDefaultAsync(u => u.Id == "1");
            userManagerMock.Setup(u => u.FindByIdAsync("1")).Returns(async () => actualData);

            var userService = new UserService(userManagerMock.Object, null,repo );

            Assert.Null(actualData.Email);
            Assert.Null(actualData.PhoneNumber);
            Assert.True(await userService.SetEmailAndPhone("newPhone","newEmail","1"));
            var changedData =await repo.All().FirstOrDefaultAsync(u=>u.Id=="1");
            Assert.Equal(changedData.Email,"newEmail");
            Assert.Equal(changedData.PhoneNumber,"newPhone");
        }
        [Fact]
        public async Task TestSetEmailAndPhoneWithImproperDataShouldReturnFalse()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var store = new Mock<IUserStore<User>>();
            var repo = new Repository<User>(context);
            var userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            userManagerMock.Setup(u => u.FindByIdAsync("1")).Returns(async () => null);

            var userService = new UserService(userManagerMock.Object, null, repo);

            Assert.False(await userService.SetEmailAndPhone("newPhone", "newEmail", "1"));
        }
    }
}
