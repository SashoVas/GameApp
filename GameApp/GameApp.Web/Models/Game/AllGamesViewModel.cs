using GameApp.Services.Models;

namespace GameApp.Web.Models.Game
{
    public class AllGamesViewModel
    {
        public IEnumerable<AllGamesServiceListingModel> Games { get; set; }
        public SearchGameInfoModel SearchInfo { get; set; }
    }
}
