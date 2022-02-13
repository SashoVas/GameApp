using GameApp.Services.Models;

namespace GameApp.Web.Models.Receipt
{
    public class AllReceiptViewModel
    {
        public IEnumerable<AllReceiptsServiceListingModel> Receipts { get; set; }
    }
}
