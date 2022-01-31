using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GameApp.Services.Models
{
    public class GameServiceListingModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ImgUrl { get; set; }
    }
}
