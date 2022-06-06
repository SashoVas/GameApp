using System.ComponentModel.DataAnnotations;

namespace GameApp.Data.Models
{
    public class Review
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public int GameId { get; set; }
        [Required]
        public Game Game { get; set; }
        [Range(1,10)]
        [Required]
        public double Score { get; set; }
        public DateTime? ReviewDate { get; set; }
    }
}
