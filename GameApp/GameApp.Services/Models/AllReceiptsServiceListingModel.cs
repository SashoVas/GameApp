using GameApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services.Models
{
    public class AllReceiptsServiceListingModel
    {
        public IEnumerable<ReceiptGameSeviceModel> Games { get; set; }
        public string Date { get; set; }
        public string Id { get; set; }
        public string CardFirstName { get; set; }
        public string CardLastName { get; set; }
        public CardType CardType { get; set; }
        public string CardNumber { get; set; }
        public ReceiptType ReceiptType { get; set; }
    }
}
