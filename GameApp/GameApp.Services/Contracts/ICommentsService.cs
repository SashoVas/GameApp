using GameApp.Services.Models;

namespace GameApp.Services.Contracts
{
    public interface ICommentsService
    {
        Task<IEnumerable<CommentsServiceListingModel>> Create(int gameId,string commentConntents, string userId);
        Task<IEnumerable<ReplyServiceListingModel>> CreateReply(int gameId,string commentConntents, string userId,string commentId);
        Task<IEnumerable<CommentsServiceListingModel>> LoadComments(int pageId, int gameId);
        Task<IEnumerable<ReplyServiceListingModel>> LoadReplies(string commentId);
        Task<bool> CommentExist(string id);

    }
}
