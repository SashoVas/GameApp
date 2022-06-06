namespace GameApp.Data.Seeding
{
    internal interface ISeeder
    {
        Task Seed(GameAppDbContext context);
    }
}
