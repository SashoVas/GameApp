using GameApp.Services.Contracts;
using GameApp.Web.Models.Comments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameApp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService commentsService;
        public CommentsController(ICommentsService commentsServicel)
        {
            this.commentsService = commentsServicel;
        }
        [HttpGet("Something")]
        public ActionResult Something()
        {
            return this.Content("hi");
        }
        [HttpPost("AddCommentToGame")]
        public async Task<ActionResult> AddCommentToGame([FromBody] AddCommentInputModel comment)
        {
            await commentsService.Create(comment.GameId,comment.Contents, this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return this.Ok();
        }

        [HttpGet("LoadComments/{page?}")]
        public async Task<ActionResult<LoadCommentsViewModel>> LoadComments(int page,int gameId)
        {
            var model = new LoadCommentsViewModel 
            { 
                Comments=await commentsService.LoadComments(page, gameId)
            };
            return model;
        }

        [HttpPost("AddReply")]
        public async Task<IActionResult>AddReply([FromBody] AddReplyInputModel reply)
        {
            await commentsService.CreateReply(reply.GameId, reply.Contents, this.User.FindFirstValue(ClaimTypes.NameIdentifier),reply.CommentId);
            return this.Ok();
        }
        [HttpGet("LoadReplies/{commentId}")]
        public async Task<ActionResult<RepliesViewModel>>LoadReplies(string commentId)
        {
            var model = new RepliesViewModel
            {
                Replies =await commentsService.LoadReplies(commentId)
            };
            return model;
        }

    }
}
