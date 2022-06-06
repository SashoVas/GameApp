using System.ComponentModel.DataAnnotations;

namespace GameApp.Data.Models
{
    public class GameGenre
    {
        [Required]
        public int GenreId { get; set; }
        [Required]
        public Genre Genre { get; set; }
        [Required]
        public int GameId { get; set; }
        [Required]
        public Game Game { get; set; }
    }
}
