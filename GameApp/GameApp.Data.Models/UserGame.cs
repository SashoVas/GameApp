using System;
using System.Collections.Generic;
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
            this.Receipts = new ICollection<ReceiptUserGame>();
        }
        public int Id { get; set; }
        public float PlayTime { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
        public ICollection<ReceiptUserGame> Receipts { get; set; }
        public DateTime Date { get; set; }
        public bool IsRefunded { get; set; }
    }
}
