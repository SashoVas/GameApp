using GameApp.Services.Contracts;
using GameApp.Web.Models.Receipt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace GameApp.Web.Controllers
{
    public class ReceiptController : Controller
    {
        private readonly IReceiptService receiptService;

        public ReceiptController(IReceiptService receiptService) 
            => this.receiptService = receiptService;
        [Authorize]
        [Route("Receipt/{id?}")]
        public async Task<IActionResult> Receipt([Required]string Id)
        {
            var receipt =await receiptService.GetReceipt(Id);
            var model = new ReceiptViewModel
            {
                Games = receipt.Games,
                Date = receipt.Date,
                CardFirstName = receipt.CardFirstName,
                CardLastName = receipt.CardLastName,
                CardNumber = receipt.CardNumber,
                CardType = receipt.CardType,
                ReceiptType = receipt.ReceiptType
            };
            return this.View(model);
        }
        [Authorize]
        [Route("Receipt/All")]
        public async Task<IActionResult> All()
        {
            var model = new AllReceiptViewModel
            {
                Receipts =await receiptService.GetAll(this.User.FindFirstValue(ClaimTypes.NameIdentifier))
            };
            if (model.Receipts==null)
            {
                this.NotFound();
            }
            return this.View(model);
        }
    }
}
