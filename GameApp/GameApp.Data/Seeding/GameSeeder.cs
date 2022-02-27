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
                for (int i = 0; i < 10; i++)
                {
                    var game = new Game
                    {
                        Name = "Game" + i.ToString(),
                        Description = "Description" + i.ToString(),
                        Price = 30,
                        ImageUrl = "User.png",
                        ReleaseDate= DateTime.UtcNow.AddDays(-(i+5)),
                        
                    };
                    game.Genres.Add(new GameGenre { Genre = context.Genres.SingleOrDefault(g => g.Name == "Action") });
                    await context.Games.AddAsync(game);
                }
                for (int i = 0; i < 5; i++)
                {
                    var game = new Game 
                    {
                        Name = "UpcomingGame" + i.ToString(),
                        Description = "UpcomingGameDescription" + i.ToString(),
                        Price = 60,
                        ImageUrl = "User.png",
                        ReleaseDate = DateTime.UtcNow.AddDays(i + 5)
                    };
                    game.Genres.Add(new GameGenre { Genre = context.Genres.SingleOrDefault(g => g.Name == "Comedy") });
                    await context.Games.AddAsync(game);
                }

            }
        }
    }
}
