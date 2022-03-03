using GameApp.Data.Models;
using GameApp.Services.Models;

namespace GameApp.Web.Models.Card
{
    public class AllCardsViewModel
    {
        public IEnumerable<AllCardsServiceListingModel> Cards { get; set; }
    }
}
