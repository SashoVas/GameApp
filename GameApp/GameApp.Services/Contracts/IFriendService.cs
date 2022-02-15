using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services.Contracts
{
    public interface IFriendService
    {
        Task<IEnumerable<string>> GetFriends(string userId);
        Task<IEnumerable<string>> GetRequests(string userId);

    }
}
