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
        [RegularExpression("^(https://www\\.youtube\\.com/watch\\?v=[A-z,0-8,\\-,_]{11})", ErrorMessage ="Youtube video expected")]
        [MaxLength(100)]
        public string? VideoUrl { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [MaxLength(100)]
        public IFormFile? Image { get; set; }
        [Required]
        public IEnumerable<string> Genres { get; set; }
    }
}
