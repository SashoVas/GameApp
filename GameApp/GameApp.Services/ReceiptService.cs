using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly UserManager<User> userManager;
        private readonly IRepository<Receipt> receipts;
        public ReceiptService(UserManager<User> userManager, IRepository<Receipt> receipts)
        {
            this.userManager = userManager;
            this.receipts = receipts;
        }
        public async Task<bool> CreateReceipt(string userId, List<UserGame> userGames)
        {
            var receipt = new Receipt
            {
                Id=Guid.NewGuid().ToString(),
                User = await userManager.FindByIdAsync(userId),
                ReceiptDate = DateTime.UtcNow
            };
            receipt.UserGames = userGames;
            await receipts.AddAsync(receipt);
            return true;
            
        }
    }
}
