using GameApp.Services.Contracts;
using GameApp.Web.Models.Friend;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameApp.Web.Infrastructure.ViewComponents
{
    
    public class FriendsViewComponent: ViewComponent
    {
        private readonly IFriendService friendService;

        public FriendsViewComponent(IFriendService friendService)
        {
            this.friendService = friendService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //Security Problem
            string userId = this.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model =new FriendViewModel
            { 
                Friends=await friendService.GetFriends(userId),
                Requests=await friendService.GetRequests(userId)
            };
            return View(model);
        }
    }
    
}
