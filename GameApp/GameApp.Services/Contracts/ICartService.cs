using GameApp.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services.Contracts
{
    public interface ICartService
    {
        Task<bool> AddToCart(int id);
        Task<bool> RemoveFromCart(int id);
        Task<IEnumerable<CartServiceListingModel>> GetAllItems();
        Task<bool> Clear();
        public Task<bool> BuyItems(string userId);
    }
}
