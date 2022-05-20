using GameApp.Data.Models;
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
        public string Video { get; set; }
        public int Popularity { get; set; }
        public double GameRating { get; set; }
        public double UserRating { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int ReceiptsCount { get; set; }
        public IEnumerable<string> Genres { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
    }
}
