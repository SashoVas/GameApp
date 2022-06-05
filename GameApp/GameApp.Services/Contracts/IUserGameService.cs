using GameApp.Services.Models;

namespace GameApp.Services.Contracts
{
    public interface IUserGameService
    {
        Task<IEnumerable<RefundableItemsServiceModel>> GetGameForRefund(string userId);
        Task<bool> RefundGame(int gameId, string userId);
    }
}
