using GameApp.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services.Contracts
{
    public interface ICommentsService
    {
        Task<bool> Create(int gameId,string commentConntents, string userId);
        Task<bool> CreateReply(int gameId,string commentConntents, string userId,string commentId);
        Task<IEnumerable<CommentsServiceListingModel>> LoadComments(int pageId, int gameId);
        Task<IEnumerable<ReplyServiceListingModel>> LoadReplies(string commentId);

    }
}
