using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Data.Models
{
    public class Receipt
    {
        public Receipt()
        {
            UserGames = new HashSet<ReceiptUserGame>();
        }
        public string Id { get; set; }
        public ICollection<ReceiptUserGame> UserGames { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string CardId { get; set; }
        public Card Card { get; set; }
        public DateTime ReceiptDate { get; set; }
        public ReceiptType ReceiptType { get; set; }

    }
}
