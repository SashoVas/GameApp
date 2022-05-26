using GameApp.Services.Contracts;
using GameApp.Web.Models.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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

        [Authorize]
        [HttpPost("AddCommentToGame")]
        public async Task<ActionResult<LoadCommentsViewModel>> AddCommentToGame([FromBody] AddCommentInputModel comment)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }
            var comments = (await commentsService.Create(comment.GameId, comment.Contents, this.User.FindFirstValue(ClaimTypes.NameIdentifier))).ToList();
            comments[0].Username = User.Identity.Name;
            var model=new LoadCommentsViewModel 
            { 
                Comments=comments
            };
            return model;
        }

        [HttpGet("LoadComments/{page?}")]
        public async Task<ActionResult<LoadCommentsViewModel>> LoadComments([Required] int page, [Required] int gameId)
        {
            var model = new LoadCommentsViewModel 
            { 
                Comments=await commentsService.LoadComments(page, gameId)
            };
            return model;
        }
        //[Authorize]
        [HttpPost("AddReply")]
        public async Task<ActionResult<RepliesViewModel>>AddReply([FromBody] AddReplyInputModel reply)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }
            var replies = (await commentsService.CreateReply(reply.GameId, reply.Contents, this.User.FindFirstValue(ClaimTypes.NameIdentifier), reply.CommentId)).ToList();
            replies[0].Username = User.Identity.Name;
            var model = new RepliesViewModel
            {
                Replies = replies
            };
            return model;
        }
        [HttpGet("LoadReplies/{commentId}")]
        public async Task<ActionResult<RepliesViewModel>>LoadReplies([Required] string commentId)
        {
            var model = new RepliesViewModel
            {
                Replies =await commentsService.LoadReplies(commentId)
            };
            return model;
        }

    }
}
