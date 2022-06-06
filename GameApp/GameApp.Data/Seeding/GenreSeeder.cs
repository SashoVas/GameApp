using GameApp.Data.Models;

namespace GameApp.Data.Seeding
{
    public class GenreSeeder : ISeeder
    {
        public async Task Seed(GameAppDbContext context)
        {
            if (!context.Genres.Any())
            {
                await context.Genres.AddAsync(new Genre { Name = "Action" });
                await context.Genres.AddAsync(new Genre { Name = "Comedy" });
                await context.Genres.AddAsync(new Genre { Name = "Adventure" });
                await context.Genres.AddAsync(new Genre { Name = "MMO" });
                await context.Genres.AddAsync(new Genre { Name = "RPG" });
                await context.Genres.AddAsync(new Genre { Name = "Multyplayer" });
                await context.Genres.AddAsync(new Genre { Name = "Singleplayer" });
            }
        }
    }
}
