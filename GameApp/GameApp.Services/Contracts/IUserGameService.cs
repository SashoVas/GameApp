using GameApp.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services.Contracts
{
    public interface IUserGameService
    {
        Task<IEnumerable<RefundableItemsServiceModel>> GetGameForRefund(string userId);
        Task<bool> RefundGame(int gameId, string userId);
    }
}
