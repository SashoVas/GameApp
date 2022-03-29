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
        private readonly IUserService userService;
        private readonly IRepository<Receipt> receipts;
        private readonly ICardService cardService;
        public ReceiptService(IUserService userService, IRepository<Receipt> receipts, ICardService cardService)
        {
            this.userService = userService;
            this.receipts = receipts;
            this.cardService = cardService;
        }
        public async Task<bool> CreateReceipt(string userId, List<UserGame> userGames,string cardId,ReceiptType receiptType)
        {
            var receipt = new Receipt
            {
                Id=Guid.NewGuid().ToString(),
                ReceiptDate = DateTime.UtcNow,
                ReceiptType=receiptType
            };
            var hasUser=await userService.SetUsersToReceipt(receipt,userId);
            if (!hasUser)
            {
                return false;
            }
            var hasCard=await cardService.SetCardToReceipt(receipt,cardId);
            if (!hasCard)
            {
                return false;
            }
            receipt.UserGames = userGames;
            await receipts.AddAsync(receipt);
            return true;
            
        }

        public async Task<IEnumerable<AllReceiptsServiceListingModel>> GetAll(string userId)
        {
            return await receipts.All()
                .Where(r => r.UserId == userId)
                .Select(r => new AllReceiptsServiceListingModel 
                { 
                   Games=r.UserGames
                   .Select(ug=> new ReceiptGameSeviceModel 
                   { 
                       Name=ug.Game.Name,
                       Price=ug.Game.Price
                   }),
                    Date = r.ReceiptDate.ToString("yyyy,MM,dd"),
                   Id=r.Id
                }).ToListAsync();
        }

        public async Task<AllReceiptsServiceListingModel> GetReceipt(string receiptId)
        {

            var receipt =await receipts
                .All()
                .Include(r=>r.Card)
                .Include(r=>r.UserGames)
                .ThenInclude(ugs=>ugs.UserGame)
                .ThenInclude(ug=>ug.Game)
                .SingleOrDefaultAsync(r => r.Id == receiptId);
            if (receipt==null)
            {
                return null;
            }
            return new AllReceiptsServiceListingModel 
            {
                Games = receipt.UserGames
                   .Select(ug => new ReceiptGameSeviceModel
                   {
                       Name = ug.Game.Name,
                       Price = ug.Game.Price
                   }),
                Date = receipt.ReceiptDate.ToString("yyyy,MM,dd"),
                Id = receipt.Id,
                CardFirstName=receipt.Card.FirstName,
                CardLastName=receipt.Card.LastName,
                CardNumber=receipt.Card.CardNumber,
                CardType=receipt.Card.CardType,
                ReceiptType=receipt.ReceiptType
            };
        }
    }
}
