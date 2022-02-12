using GameApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services.Contracts
{
    public interface IReceiptService
    {
        Task<bool> CreateReceipt(string userId, List<UserGame> userGames);
    }
}
