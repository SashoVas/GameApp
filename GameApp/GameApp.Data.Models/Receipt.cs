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
            UserGames = new HashSet<UserGame>();
        }
        public string Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<UserGame> UserGames { get; set; }
        public DateTime ReceiptDate { get; set; }

    }
}
