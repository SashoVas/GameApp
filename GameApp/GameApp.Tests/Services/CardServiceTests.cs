using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services;
using GameApp.Tests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GameApp.Tests.Services
{
    public class CardServiceTests
    {
        private List<Card>GetDummyData()
        {
            var user1 = new User
            {
                UserName = "1",
                Id = "1"
            };

            var user2 = new User
            {
                UserName = "2",
                Id = "2",
            };
            var cards = new List<Card>();
            for (int i = 1; i < 16; i++)
            {
                var card = new Card
                {

                    Id = "Card" + i.ToString(),
                    Address = "Card" + i.ToString(),
                    CardNumber = "Card" + i.ToString(),
                    CardType = CardType.MasterCard,
                    City = "Card" + i.ToString(),
                    Country = "Card" + i.ToString(),
                    ExpirationDate = DateTime.UtcNow,
                    FirstName = "Card" + i.ToString(),
                    LastName = "Card" + i.ToString(),
                    PhoneNumber = "Card" + i.ToString(),
                    ZipCode = "Card" + i.ToString(),
                    User = i % 2 == 0 ? user1 : user2,
                };
                cards.Add(card);
            }
            return cards;
        }
        
        private async Task SeedData(GameAppDbContext context)
        {
            await context.AddRangeAsync(GetDummyData());
            await context.SaveChangesAsync();
        }
        [Fact]
        public async Task TestGetAllCardsShouldReturnAllCards()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Card>(context);
            var cardService = new CardService(repo);

            var result =(await cardService.GetCards("1")).ToList();
            var actual =await repo.All().Where(c=>c.UserId=="1").ToListAsync();

            for (int i = 0; i < result.Count(); i++)
            {
                Assert.Equal(result[i].FirstName,actual[i].FirstName);
                Assert.Equal(result[i].LastName,actual[i].LastName);
                Assert.Equal(result[i].Id,actual[i].Id);
                Assert.Equal(result[i].CardType,actual[i].CardType);
            }
        }
        [Fact]
        public async Task TestGetAllCardsWithImproperDataShouldReturnEmpty()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Card>(context);
            var cardService = new CardService(repo);

            var result = (await cardService.GetCards("not a value")).ToList();
            Assert.Empty(result);
        }
        [Fact]
        public async Task TestGetCardShoutReturnCard()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Card>(context);
            var cardService = new CardService(repo);

            var result =await cardService.GetCard("2", "Card1");
            var acctual =await repo.All().FirstOrDefaultAsync(c=>c.UserId=="2"&&c.Id=="Card1");

            Assert.Equal(result.FirstName, acctual.FirstName);
            Assert.Equal(result.LastName, acctual.LastName);
            Assert.Equal(result.Country, acctual.Country);
            Assert.Equal(result.City, acctual.City);
            Assert.Equal(result.PhoneNumber, acctual.PhoneNumber);
            Assert.Equal(result.CardNumber, acctual.CardNumber);
            Assert.Equal(result.CardType, acctual.CardType);
            Assert.Equal(result.Address, acctual.Address);
            Assert.Equal(result.ExpirationDate, acctual.ExpirationDate);
            Assert.Equal(result.ZipCode, acctual.ZipCode);
        }
        [Fact]
        public async Task TestGetCardWithImproperDataShoutNull()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Card>(context);
            var cardService = new CardService(repo);

            var result = await cardService.GetCard("not a value", "not a value");

            Assert.Null(result);

            result = await cardService.GetCard(null, null);

            Assert.Null(result);
        }

        [Fact]
        public async Task TestCreateCardShouldCreateCard()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Card>(context);
            var cardService = new CardService(repo);
            var card = new Card
            {

                Id = "NewCard" ,
                Address = "NewCardAdders" ,
                CardNumber = "NewCardCardNumber",
                CardType = CardType.MasterCard,
                City = "NewCardCity",
                Country = "NewCardCountry",
                ExpirationDate = DateTime.UtcNow,
                FirstName = "NewCardFirstName",
                LastName = "NewCardLastName",
                PhoneNumber = "NewCardPhoneNumber",
                ZipCode = "NewCardZipCode",
                UserId = "1"
            };

            await cardService.Create(card.CardType,card.CardNumber,card.FirstName,card.LastName,card.Address,card.Country,card.ExpirationDate,card.City,card.ZipCode,card.PhoneNumber,"1");
            var result = repo.All().Last();
            Assert.Equal(result.Address, card.Address);
            Assert.Equal(result.CardNumber, card.CardNumber);
            Assert.Equal(result.CardType, card.CardType);
            Assert.Equal(result.City, card.City);
            Assert.Equal(result.Country, card.Country);
            Assert.Equal(result.ExpirationDate, card.ExpirationDate);
            Assert.Equal(result.FirstName, card.FirstName);
            Assert.Equal(result.LastName, card.LastName);
            Assert.Equal(result.PhoneNumber, card.PhoneNumber);
            Assert.Equal(result.ZipCode, card.ZipCode);
            Assert.Equal(result.UserId, card.UserId);
        }
        [Fact]
        public async Task TestRemoveCardShouldRemoveCard()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Card>(context);
            var cardService = new CardService(repo);
            context.ChangeTracker.Clear();
            Assert.True(await cardService.Remove("Card1"));
            Assert.Null(await repo.All().FirstOrDefaultAsync(c => c.Id == "Card1"));
        }
    }
}
