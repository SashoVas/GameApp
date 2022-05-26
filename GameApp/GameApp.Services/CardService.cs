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
        public CardService(IRepository<Card> cards) 
            => this.cards = cards;
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
                UserId=userId
            };
            await cards.AddAsync(card);
            await cards.SaveChangesAsync();
            return true;
        }

        public async Task<CardServiceModel> GetCard(string userId, string cardId) 
            => await cards.All()
                .Where(c => c.UserId == userId && c.Id == cardId)
                .Select(c => new CardServiceModel
                {
                    FirstName = c.FirstName,
                    CardType = c.CardType,
                    CardNumber = c.CardNumber,
                    Address = c.Address,
                    City = c.City,
                    Country = c.Country,
                    ZipCode = c.ZipCode,
                    ExpirationDate = c.ExpirationDate,
                    LastName = c.LastName,
                    PhoneNumber = c.PhoneNumber
                }).FirstOrDefaultAsync();

        public async Task<IEnumerable<AllCardsServiceListingModel>> GetCards(string userId) 
            => await cards
                .All()
                .Where(c => c.UserId == userId)
                .Select(c => new AllCardsServiceListingModel
                {
                    Id = c.Id,
                    CardType = c.CardType,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    CardNumber = c.CardNumber
                }).ToListAsync();

        public async Task<bool> Remove(string cardId)
        {
            var card = new Card 
            { 
                Id = cardId 
            };
            cards.Delete(card);
            await cards.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetCard(CardType cardType, string cardNumber, string firstName, string lastName, string address, string country, DateTime date, string city, string zipCode, string phoneNumber, string userId,string cardId)
        {
            var card =await cards.All().FirstOrDefaultAsync(c=>c.UserId==userId&&c.Id==cardId);
            card.FirstName = firstName;
            card.LastName = lastName;
            card.CardNumber = cardNumber;
            card.Address = address;
            card.Country = country;
            card.ExpirationDate = date;
            card.City = city;
            card.ZipCode = zipCode;
            card.PhoneNumber = phoneNumber;
            card.CardType = cardType;
            cards.Update(card);
            await cards.SaveChangesAsync();
            return true;
        }
    }
}
