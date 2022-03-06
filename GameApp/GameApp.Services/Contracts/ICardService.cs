using GameApp.Data.Models;
using GameApp.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Task<bool> HaveCard(string userId);
        Task<bool> Remove(string cardId);
        Task<IEnumerable<AllCardsServiceListingModel>> GetCards(string userId);
        Task<bool> SetCardToReceipt(Receipt receipt,string cardId);
    }
}
