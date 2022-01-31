using GameApp.Services.Models;

namespace GameApp.Web.Models.Cart
{
    public class AllItemsInCartViewModel
    {
        public IEnumerable<CartServiceListingModel>Items { get; set; }
    }
}
