using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using GameApp.Services.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IRepository<Receipt> receipts;
        public ReceiptService(IRepository<Receipt> receipts) 
            => this.receipts = receipts;
        public virtual async Task<bool> CreateReceipt(string userId, List<UserGame> userGames,string cardId,ReceiptType receiptType)
        {
            var receipt = new Receipt
            {
                Id=Guid.NewGuid().ToString(),
                ReceiptDate = DateTime.UtcNow,
                ReceiptType=receiptType,
                CardId=cardId,
            };
            receipt.UserGames = userGames.Select(ug=>new ReceiptUserGame
            {
                Receipt=receipt,
                UserGame=ug,
                Id=Guid.NewGuid().ToString(),
            }).ToList();
            await receipts.AddAsync(receipt);
            return true;
        }

        public async Task<IEnumerable<AllReceiptsServiceListingModel>> GetAll(string userId) 
            => await receipts
                .All()
                .Include(r => r.UserGames)
                .ThenInclude(ugs => ugs.UserGame)
                .Where(r => r.UserGames.FirstOrDefault().UserGame.UserId == userId)
                .Select(r => new AllReceiptsServiceListingModel
                {
                    Games = r.UserGames
                   .Select(ug => new ReceiptGameSeviceModel
                   {
                       Name = ug.UserGame.Game.Name,
                       Price = ug.UserGame.Game.Price
                   }),
                    Date = r.ReceiptDate.ToString("yyyy,MM,dd"),
                    Id = r.Id
                }).ToListAsync();

        public async Task<AllReceiptsServiceListingModel> GetReceipt(string receiptId) 
            => await receipts.All()
                .Where(r => r.Id == receiptId)
                .Select(r => new AllReceiptsServiceListingModel
                {
                    Games = r.UserGames
                   .Select(ug => new ReceiptGameSeviceModel
                   {
                       Name = ug.UserGame.Game.Name,
                       Price = ug.UserGame.Game.Price
                   }),
                    Date = r.ReceiptDate.ToString("yyyy,MM,dd"),
                    Id = r.Id,

                    CardFirstName = r.Card != null ? r.Card.FirstName : null,
                    CardLastName = r.Card != null ? r.Card.LastName : null,
                    CardNumber = r.Card != null ? r.Card.CardNumber : null,
                    CardType = r.Card != null ? r.Card.CardType : CardType.PayPal,
                    ReceiptType = r.ReceiptType
                }).FirstOrDefaultAsync();
    }
}
