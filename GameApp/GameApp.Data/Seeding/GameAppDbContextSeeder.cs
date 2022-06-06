namespace GameApp.Data.Seeding
{
    public class GameAppDbContextSeeder : ISeeder
    {
        public async Task Seed(GameAppDbContext context)
        {
            var seeders = new List<ISeeder> 
            { 
                new RoleSeeder()
                ,new GenreSeeder()
                ,new GameSeeder() 
            };
            foreach (var seeder in seeders)
            {
                await seeder.Seed(context);
                await context.SaveChangesAsync();
            }
        }
    }
}
