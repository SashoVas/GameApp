using GameApp.Services.Contracts;
using GameApp.Web.Models.Friend;
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
        [HttpGet("Example")]
        public async Task<ActionResult> Example()
        {
            return this.Content("hi");
        }
        [HttpPost("SendFirendRequest")]
        public async Task<ActionResult> SendFirendRequest([FromBody] FriendRequestInputModel friend)
        {
            await friendService.SendFriendRequest(this.User.FindFirstValue(ClaimTypes.NameIdentifier), friend.Username);
            return this.Ok();
        }

        [HttpPost("AcceptFirendRequest")]
        public async Task<ActionResult> AcceptFirendRequest([FromBody] FriendRequestInputModel friend)
        {
            //Security Problem
            await friendService.ChangeStatus(this.User.FindFirstValue(ClaimTypes.NameIdentifier), friend.Username, "Friend");
            return this.Ok();
        }

        [HttpPost("RejectFirendRequest")]
        public async Task<ActionResult> RejectFirendRequest([FromBody] FriendRequestInputModel friend)
        {
            //Security Problem
            await friendService.ChangeStatus(this.User.FindFirstValue(ClaimTypes.NameIdentifier), friend.Username, "Rejected");
            return this.Ok();
        }
    }
}
