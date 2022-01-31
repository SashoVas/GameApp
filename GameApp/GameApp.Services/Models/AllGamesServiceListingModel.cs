using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services.Models
{
    public class AllGamesServiceListingModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public IEnumerable<string> Genres { get; set; }
        public string ImgUrl { get; set; }
    }
}
