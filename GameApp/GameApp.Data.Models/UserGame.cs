using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Data.Models
{
    public class UserGame
    {
        //public float Rating { get; set; }
        public UserGame()
        {
            this.Date = DateTime.Now;
            this.Receipts = new HashSet<ReceiptUserGame>();
        }
        public int Id { get; set; }
        [Required]
        public float PlayTime { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public int GameId { get; set; }
        [Required]
        public Game Game { get; set; }
        public ICollection<ReceiptUserGame> Receipts { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public bool IsRefunded { get; set; }
    }
}
