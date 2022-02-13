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
    }
}
