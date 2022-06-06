using System.ComponentModel.DataAnnotations;

namespace GameApp.Data.Models
{
    public class Genre
    {
        public Genre()
        {
            Games=new HashSet<GameGenre>();
        }
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public ICollection<GameGenre> Games { get; set; }
    }
}
