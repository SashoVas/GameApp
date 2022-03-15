using GameApp.Data.Models;
using GameApp.Services.Contracts;
using GameApp.Web.Models.Friend;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace GameApp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService friendService;

        public FriendController(IFriendService friendService)
        {
            this.friendService = friendService;
        }
        [Authorize]
        [HttpPost("SendFirendRequest")]
        public async Task<ActionResult> SendFirendRequest([FromBody] FriendRequestInputModel friend)
        {
            if (this.User.Identity.Name==friend.Username)
            {
                return this.BadRequest();
            }
            var hasFriend=await friendService.SendFriendRequest(this.User.FindFirstValue(ClaimTypes.NameIdentifier), friend.Username);
            if (!hasFriend)
            {
                return this.BadRequest();
            }
            return this.Ok();
        }
        [Authorize]
        [HttpPost("AcceptFirendRequest")]
        public async Task<ActionResult> AcceptFirendRequest([FromBody] FriendRequestInputModel friend)
        {
            //Security Problem
            await friendService.ChangeStatus(this.User.FindFirstValue(ClaimTypes.NameIdentifier), friend.Username, FriendStatus.Friend, FriendStatus.Request);
            return this.Ok();
        }
        [Authorize]
        [HttpPost("RejectFirendRequest")]
        public async Task<ActionResult> RejectFirendRequest([FromBody] FriendRequestInputModel friend)
        {
            //Security Problem
            await friendService.ChangeStatus(this.User.FindFirstValue(ClaimTypes.NameIdentifier), friend.Username, FriendStatus.Rejected, FriendStatus.Request);
            return this.Ok();
        }
        [Authorize]
        [HttpPost("Unfriend")]
        public async Task<ActionResult> Unfriend([FromBody] FriendRequestInputModel friend)
        {
            //Security Problem
            await friendService.ChangeStatus(this.User.FindFirstValue(ClaimTypes.NameIdentifier), friend.Username, FriendStatus.Rejected, FriendStatus.Friend);
            return this.Ok();
        }
    }
}
