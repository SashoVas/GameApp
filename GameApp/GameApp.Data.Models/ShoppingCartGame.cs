using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Data.Models
{
    public class ShoppingCartGame
    {
        public string Id { get; set; }
        public string ShoppingCartId { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}
