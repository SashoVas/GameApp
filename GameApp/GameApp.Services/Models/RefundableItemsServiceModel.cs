using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services.Models
{
    public class RefundableItemsServiceModel
    {
        public string IMG { get; set; }
        public int GameId { get; set; }
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
