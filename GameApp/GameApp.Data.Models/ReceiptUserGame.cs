using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Data.Models
{
    public class ReceiptUserGame
    {
        public string Id { get; set; }
        public UserGame UserGame { get; set; }
        public int UserGameId { get; set; }
        public Receipt Receipt { get; set; }
        public string ReceiptId { get; set; }
    }
}
