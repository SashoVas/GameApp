using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Data.Models
{
    public class Game
    {
        public Game()
        {
            Users = new HashSet<UserGame>();
            ShoppingCartGames = new HashSet<ShoppingCartGame>();
            Genres = new HashSet<GameGenre>();
            Comments=new HashSet<Comment>();
            Reviews = new HashSet<Review>();
        }
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public decimal Price { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        [MaxLength(100)]
        public string? ImageUrl { get; set; }
        [MaxLength(100)]
        public string? Video { get; set; }
        public DateTime ReleaseDate { get; set; }
        public ICollection<GameGenre> Genres { get; set; }
        public ICollection<UserGame> Users { get; set; }
        public ICollection<ShoppingCartGame> ShoppingCartGames { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
