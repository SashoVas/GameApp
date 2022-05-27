using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public string? CardId { get; set; }
        public Card? Card { get; set; }
        [Required]
        public DateTime ReceiptDate { get; set; }
        [Required]
        public ReceiptType ReceiptType { get; set; }

    }
}
