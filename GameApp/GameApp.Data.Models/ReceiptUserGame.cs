using System.ComponentModel.DataAnnotations;

namespace GameApp.Data.Models
{
    public class ReceiptUserGame
    {
        public string Id { get; set; }
        [Required]
        public UserGame UserGame { get; set; }
        [Required]
        public int UserGameId { get; set; }
        [Required]
        public Receipt Receipt { get; set; }
        [Required]
        public string ReceiptId { get; set; }
    }
}
