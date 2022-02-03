using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Data.Models
{
    public class GameGenre
    {
        public float Rating { get; set; }
        public float PlayTime { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}
