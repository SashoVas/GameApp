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
        public bool HaveGame { get; set; }
        public int Users { get; set; }
        public int Rank { get; set; }
        public int Popularity { get; set; }
        public float GameRating { get; set; }
        public float UserRating { get; set; }
        public DateTime ReleaseDate { get; set; }
        public IEnumerable<string> Genres { get; set; }
    }
}
