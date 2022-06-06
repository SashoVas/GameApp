using System.ComponentModel.DataAnnotations;

namespace GameApp.Data.Models
{
    public class ShoppingCartGame
    {
        public string Id { get; set; }
        [Required]
        public string ShoppingCartId { get; set; }
        [Required]
        public int GameId { get; set; }
        [Required]
        public Game Game { get; set; }
    }
}
