using GameApp.Data.Models;
using GameApp.Services.Models;

namespace GameApp.Services.Contracts
{
    public interface ICardService
    {
        Task<bool> Create(CardType cardType,
            string cardNumber,
            string firstName,
            string lastName,
            string address,
            string country,
            DateTime date,
            string city,
            string zipCode,
            string phoneNumber,
            string userId
            );
        Task<bool> SetCard(CardType cardType,
            string cardNumber,
            string firstName,
            string lastName,
            string address,
            string country,
            DateTime date,
            string city,
            string zipCode,
            string phoneNumber,
            string userId,
            string cardId
            );
        Task<CardServiceModel> GetCard(string userId, string cardId);
        Task<bool> Remove(string cardId);
        Task<IEnumerable<AllCardsServiceListingModel>> GetCards(string userId);
        Task<bool> CardExist(string id);
    }
}
