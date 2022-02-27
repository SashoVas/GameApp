using GameApp.Services.Models;

namespace GameApp.Web.Models.Home
{
    public class HomeIndexViewModel
    {
        public PopularGamesServiceListingModel[] PopularGames { get; set; }
        public PopularGamesServiceListingModel[] TopRankedGames { get; set; }
        public PopularGamesServiceListingModel[] UpcomingGames { get; set; }
    }
}
