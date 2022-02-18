using System.ComponentModel.DataAnnotations;

namespace GameApp.Web.Models.Game
{
    public class CreateGameInputModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string Name { get; set; }
        [Range(0.99,double.MaxValue)]
        public decimal Price { get; set; }
        [MaxLength(500)]
        [Required]
        [MinLength(3)]
        public string Description { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public IFormFile? Image { get; set; }
        [Required]
        public IEnumerable<string> Genres { get; set; }
    }
}
