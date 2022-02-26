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
        [HttpGet("Something")]
        public ActionResult Something()
        {
            return this.Content("hi");
        }
        [Authorize]
        [HttpPost("AddCommentToGame")]
        public async Task<ActionResult<LoadCommentsViewModel>> AddCommentToGame([FromBody] AddCommentInputModel comment)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }
            var model=new LoadCommentsViewModel 
            { 
                Comments=await commentsService.Create(comment.GameId,comment.Contents, this.User.FindFirstValue(ClaimTypes.NameIdentifier))
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
            var model = new RepliesViewModel 
            { 
                Replies=await commentsService.CreateReply(reply.GameId, reply.Contents, this.User.FindFirstValue(ClaimTypes.NameIdentifier),reply.CommentId)
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
