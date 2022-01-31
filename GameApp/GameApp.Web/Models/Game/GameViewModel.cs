using GameApp.Web.Models.Game;

namespace GameApp.Web.Models.Game
{
    public class GameViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string  ImageUrl { get; set; }
        public AllGamesViewModel SimilarGames { get; set; }
    }
}
