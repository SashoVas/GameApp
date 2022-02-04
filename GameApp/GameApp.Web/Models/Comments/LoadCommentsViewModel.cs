using GameApp.Services.Models;

namespace GameApp.Web.Models.Comments
{
    public class LoadCommentsViewModel
    {
        public IEnumerable<CommentsServiceListingModel> Comments { get; set; }

    }
}
