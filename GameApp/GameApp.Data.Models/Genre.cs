using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Data.Models
{
    public class Genre
    {
        public Genre()
        {
            Games=new HashSet<GameGenre>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<GameGenre> Games { get; set; }
    }
}
