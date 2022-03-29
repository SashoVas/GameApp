using GameApp.Data.Models;
using GameApp.Services.Models;

namespace GameApp.Web.Models.Receipt
{
    public class ReceiptViewModel
    {
        public IEnumerable<ReceiptGameSeviceModel> Games { get; set; }
        public string Date { get; set; }
        public string CardFirstName { get; set; }
        public string CardLastName { get; set; }
        public CardType CardType { get; set; }
        public string CardNumber { get; set; }
        public ReceiptType ReceiptType { get; set; }
    }
}
