using GameApp.Web.Models.Game;

namespace GameApp.Web.Models.Game
{
    public class GameViewModel
    {
        public int Id { get; set; }
        public double UserRating { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string  ImageUrl { get; set; }
        public bool HaveGame { get; set; }
        public int Users { get; set; }
        public int Rating { get; set; }
        public int Rank { get; set; }
        public double Score { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Popularity { get; set; }
        public IEnumerable<string> Genres { get; set; }
        public AllGamesViewModel SimilarGames { get; set; }
    }
}
