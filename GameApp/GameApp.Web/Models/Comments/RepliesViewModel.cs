using GameApp.Services.Models;

namespace GameApp.Web.Models.Comments
{
    public class RepliesViewModel
    {
        public IEnumerable<ReplyServiceListingModel> Replies { get; set; }
    }
}
