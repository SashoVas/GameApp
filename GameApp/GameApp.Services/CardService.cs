using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using GameApp.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services
{
    public class CardService : ICardService
    {
        private readonly IRepository<Card> cards;
        private readonly IUserService userService;
        public CardService(IRepository<Card> cards, IUserService userService)
        {
            this.cards = cards;
            this.userService = userService;
        }
        public async Task<bool> Create(CardType cardType,string cardNumber,string firstName,  string lastName, string address,  string country, DateTime date, string city, string zipCode,string phoneNumber,string userId)
        {
            
            var card = new Card 
            { 
                FirstName=firstName,
                LastName=lastName,
                CardNumber=cardNumber,
                Address=address,    
                Country=country,
                ExpirationDate=date,
                City=city,
                ZipCode=zipCode,
                PhoneNumber=phoneNumber,
                CardType=cardType,
                Id=Guid.NewGuid().ToString(),
            };
            await userService.SetUsersToCard(card,userId);
            await cards.AddAsync(card);
            await cards.SaveChangesAsync();
            return true;
        }

        public async Task<CardServiceModel> GetCard(string userId)
        {
            var card = await cards.All().FirstOrDefaultAsync(c => c.UserId == userId);
            if (card==null)
            {
                return null;
            }
            return new CardServiceModel 
            { 
                FirstName=card.FirstName,
                CardType=card.CardType,
                CardNumber=card.CardNumber,
                Address=card.Address,
                City=card.City,
                Country=card.Country,
                ZipCode=card.ZipCode,
                ExpirationDate=card.ExpirationDate,
                LastName=card.LastName,
                PhoneNumber=card.PhoneNumber
            } ;
        }

        public async Task<bool> HaveCard(string userId)
        {
            return await cards
                .All()
                .AnyAsync(c=>c.UserId==userId);
        }
    }
}
