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
                        Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry." +
                        " It has survived not only five centuries, but also the leap into electronic typesetting," +
                        " remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages" +
                        ", and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum." + i.ToString(),
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
