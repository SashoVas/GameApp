using GameApp.Services.Contracts;
using GameApp.Web.Models.Card;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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

            return this.Redirect(nameof(PaymentMethods));
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
                PhoneNumber = card.PhoneNumber,
                CardId=cardId
            });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SetCard(SetCardInputModel model)
        {
            //TODO:Set New Values
            if (!ModelState.IsValid || model.CardId==null)
            {
                return View(model);
            }
            await cardService.SetCard(model.CardType,
                model.CardNumber,
                model.FirstName,
                model.LastName,
                model.Address,
                model.Country,
                model.ExpirationDate,
                model.City,
                model.ZipCode,
                model.PhoneNumber,
                this.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ,model.CardId);

            return this.Redirect("/");
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult>Remove([Required]string cardId)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }
            await cardService.Remove(cardId);
            return this.Redirect(nameof(PaymentMethods));
        }
        public async Task <IActionResult> PaymentMethods()
        {
            var cards = new AllCardsViewModel
            {
                Cards = await cardService.GetCards(this.User.FindFirstValue(ClaimTypes.NameIdentifier))
            };
            return this.View(cards);
        }
        public async Task<IActionResult> ChooseCard()
        {
            var cards = new AllCardsViewModel
            {
                Cards = await cardService.GetCards(this.User.FindFirstValue(ClaimTypes.NameIdentifier))
            };
            if (cards.Cards.Count()==0)
            {
                return this.Redirect(nameof(CreateCard));
            }
            return this.View(cards);
        }
    }
}
