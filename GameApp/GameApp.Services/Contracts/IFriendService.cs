using GameApp.Data.Models;

namespace GameApp.Services.Contracts
{
    public interface IFriendService
    {
        Task<IEnumerable<string>> GetFriends(string userId);
        Task<IEnumerable<string>> GetRequests(string userId);
        Task<bool> SendFriendRequest(string userId,string username);
        Task<bool> ChangeStatus(string userId, string username, FriendStatus status, FriendStatus only);
    }
}
