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
            var hasUser=await userService.SetUsersToCard(card,userId);
            if (!hasUser)
            {
                return false;
            }
            await cards.AddAsync(card);
            await cards.SaveChangesAsync();
            return true;
        }

        public async Task<CardServiceModel> GetCard(string userId,string cardId)
        {
            var card = await cards.All().FirstOrDefaultAsync(c => c.UserId == userId&& c.Id== cardId);
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

        public async Task<IEnumerable< AllCardsServiceListingModel>> GetCards(string userId)
        {
            return await cards
                .All()
                .Where(c => c.UserId == userId)
                .Select(c=>new AllCardsServiceListingModel 
                { 
                    Id=c.Id,
                    CardType=c.CardType,
                    FirstName=c.FirstName,
                    LastName=c.LastName
                }).ToListAsync();
        }

        public async Task<bool> HaveCard(string userId)
        {
            return await cards
                .All()
                .AnyAsync(c=>c.UserId==userId);
        }

        public async Task<bool> Remove(string cardId)
        {
            cards.Delete(await cards.All().FirstOrDefaultAsync(c=>c.Id==cardId));
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

        public async Task<bool> SetCardToReceipt(Receipt receipt, string cardId)
        {
            var card=await cards
                .All()
                .FirstOrDefaultAsync(c=>c.Id==cardId);
            if (card==null)
            {
                return false;
            }
            receipt.Card = card;
            return true;
        }
    }
}
