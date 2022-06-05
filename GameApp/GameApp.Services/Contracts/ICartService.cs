using GameApp.Services.Models;

namespace GameApp.Services.Contracts
{
    public interface ICartService
    {
        Task<bool> AddToCart(int id);
        Task<bool> RemoveFromCart(int id);
        Task<IEnumerable<CartServiceListingModel>> GetAllItems();
        Task<bool> Clear();
        Task<bool> BuyItems(string userId,string cardId);
    }
}
