using GameApp.Services.Models;

namespace GameApp.Web.Models.Receipt
{
    public class ReceiptViewModel
    {
        public IEnumerable<ReceiptGameSeviceModel> Games { get; set; }
        public string Date { get; set; }
    }
}
