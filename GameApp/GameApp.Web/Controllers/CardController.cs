using GameApp.Services.Contracts;
using GameApp.Web.Models.Card;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameApp.Web.Controllers
{
    public class CardController : Controller
    {
        private readonly ICardService cardService;
        public CardController(ICardService cardService)
        {
            this.cardService = cardService;
        }
        [Authorize]
        public async Task<IActionResult>CreateCard()
        {

            return this.View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult>CreateCard(SetCardInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            await cardService.Create(model.CardType,
                model.CardNumber,
                model.FirstName,
                model.LastName,
                model.Address,
                model.Country,
                model.ExpirationDate,
                model.City,
                model.ZipCode,
                model.PhoneNumber,
                this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            return this.Redirect("~/Card/ChooseCard");
        }
        [Authorize]
        public async Task<IActionResult> SetCard(string cardId)
        {
            var card =await cardService.GetCard(this.User.FindFirstValue(ClaimTypes.NameIdentifier),cardId);
            if (card==null)
            {
                return this.View();
            }
            return View(new SetCardInputModel 
            {
                FirstName = card.FirstName,
                CardType = card.CardType,
                CardNumber = card.CardNumber,
                Address = card.Address,
                City = card.City,
                Country = card.Country,
                ZipCode = card.ZipCode,
                ExpirationDate = card.ExpirationDate,
                LastName = card.LastName,
                PhoneNumber = card.PhoneNumber
            });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SetCard(SetCardInputModel model)
        {
            //TODO:Set New Values
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            await cardService.Create(model.CardType,
                model.CardNumber,
                model.FirstName,
                model.LastName,
                model.Address,
                model.Country,
                model.ExpirationDate,
                model.City,
                model.ZipCode,
                model.PhoneNumber,
                this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            return this.Redirect("/");
        }

        public async Task <IActionResult> ChooseCard()
        {
            var cards = new AllCardsViewModel
            {
                Cards = await cardService.GetCards(this.User.FindFirstValue(ClaimTypes.NameIdentifier))
            };
            return this.View(cards);
        }
    }
}
