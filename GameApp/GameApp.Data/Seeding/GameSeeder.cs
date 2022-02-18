using GameApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Data.Seeding
{
    public class GameSeeder : ISeeder
    {
        public async Task Seed(GameAppDbContext context)
        {
            if (!context.Games.Any())
            {
                for (int i = 0; i < 16; i++)
                {
                    var game = new Game
                    {
                        Name = "Game" + i.ToString(),
                        Description = "Description" + i.ToString(),
                        Price = 324,
                        ImageUrl = "User.png",

                    };
                    game.Genres.Add(new GameGenre { Genre = context.Genres.SingleOrDefault(g => g.Name == "Action") });
                    await context.Games.AddAsync(game);

                }

            }
        }
    }
}
