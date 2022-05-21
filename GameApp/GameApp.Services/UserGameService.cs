using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using GameApp.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services
{
    public class UserGameService : IUserGameService
    {
        private readonly IRepository<UserGame> userGames;
        private readonly IReceiptService receiptService;

        public UserGameService(IRepository<UserGame> userGames, IReceiptService receiptService)
        {
            this.userGames = userGames;
            this.receiptService = receiptService;
        }

        public async Task<IEnumerable<RefundableItemsServiceModel>> GetGameForRefund(string userId) 
            => await userGames
                .All()
                .Include(ug => ug.Game)
                .Where(ug => ug.UserId == userId
                && ug.Date > DateTime.Now.AddDays(-3)
                && ug.IsRefunded == false)
                .Select(g => new RefundableItemsServiceModel
                {
                    IMG = g.Game.ImageUrl,
                    Name = g.Game.Name,
                    ReleaseDate = g.Game.ReleaseDate,
                    GameId = g.Game.Id,
                }).ToListAsync();

        public async Task<bool> RefundGame(int gameId, string userId)
        {
            var userGame =await userGames
                .All()
                .Include(ug=>ug.Receipts)
                .ThenInclude(ug=>ug.Receipt)
                .Include(ug=>ug.Game)
                .OrderBy(ug=>ug.Date)
                .LastOrDefaultAsync(ug=>ug.GameId==gameId&&ug.UserId==userId);
            
            if (userGame == null)
            {
                return false;
            }
            userGame.IsRefunded = true;
            userGames.Update(userGame);
            var success =await receiptService.CreateReceipt(userId,new List<UserGame> { userGame },userGame.Receipts.LastOrDefault().Receipt.CardId,ReceiptType.Refund);
            if (!success)
            {
                return false;
            }
            await userGames.SaveChangesAsync();
            return true;

        }
    }
}
