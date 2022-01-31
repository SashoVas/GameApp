﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Data.Models
{
    public class Game
    {
        public Game()
        {
            Users = new HashSet<UserGame>();
            ShoppingCartGames = new HashSet<ShoppingCartGame>();
            Genres = new HashSet<GameGenre>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<GameGenre> Genres { get; set; }
        public ICollection<UserGame> Users { get; set; }
        public ICollection<ShoppingCartGame> ShoppingCartGames { get; set; }
    }
}