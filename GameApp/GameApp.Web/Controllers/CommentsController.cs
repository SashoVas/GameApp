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
        private readonly ICommentsService commentsServicel;
        public CommentsController(ICommentsService commentsServicel)
        {
            this.commentsServicel = commentsServicel;
        }
        [HttpGet("Something")]
        public ActionResult Something()
        {
            return this.Content("hi");
        }
        [HttpPost("AddCommentToGame")]
        public async Task<ActionResult> AddCommentToGame([FromBody] AddCommentInputModel comment)
        {
            await commentsServicel.Create(comment.GameId,comment.Contents, this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return this.Ok();
        }

        [HttpGet("LoadComments/{page?}")]
        public async Task<ActionResult<LoadCommentsViewModel>> LoadComments(int page,int gameId)
        {
            var model = new LoadCommentsViewModel 
            { 
                Comments=await commentsServicel.LoadComments(page, gameId)
            };
            return model;
        }

    }
}
