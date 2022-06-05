using GameApp.Data.Models;
using GameApp.Services.Models;

namespace GameApp.Services.Contracts
{
    public interface IReceiptService
    {
        Task<bool> CreateReceipt(string userId, List<UserGame> userGames,string cardId, ReceiptType receiptType);
        Task<IEnumerable<AllReceiptsServiceListingModel>> GetAll(string userId);
        Task<AllReceiptsServiceListingModel> GetReceipt(string receiptId);
    }
}
