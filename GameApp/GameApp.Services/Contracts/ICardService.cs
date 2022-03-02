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
        Task<CardServiceModel> GetCard(string userId);
        Task<bool> HaveCard(string userId);
    }
}
