using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Data.Seeding;
using GameApp.Services;
using GameApp.Services.Contracts;
using Microsoft.AspNetCore.Identity;

namespace GameApp.Web.Infrastructure
{
    public static class ApplicationBuilderExtensions
    {
        public static void AddSession(this WebApplicationBuilder builder)
        {
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }
        public static void AddIdentity(this WebApplicationBuilder builder)
        {
            builder.Services.AddDefaultIdentity<User>(options =>
            {
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;

            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<GameAppDbContext>();
        }
        public static void AddServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddTransient<IGameService, GameService>();
            builder.Services.AddTransient<ICartService, CartService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IGenreService, GenreService>();
            builder.Services.AddTransient<ICommentsService, CommentsService>();
            builder.Services.AddTransient<IReviewService, ReviewService>();
            builder.Services.AddTransient<IReceiptService, ReceiptService>();
            builder.Services.AddTransient<IFriendService, FriendService>();
            builder.Services.AddTransient<ICardService, CardService>();
            builder.Services.AddTransient<IUserGameService, UserGameService>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddScoped(sp => ShoppingCart.GetShoppingCart(sp));
        }
        public static void SeedDataBase(this WebApplication app)
        {
            using (var serviceScope = app.Services.CreateScope())//app.ApplicationServices.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetRequiredService<GameAppDbContext>())
                {
                    context.Database.EnsureCreated();
                    new GameAppDbContextSeeder().Seed(context).GetAwaiter().GetResult();
                }
            }
        }
        public static void ConfigureEndpoints(this WebApplication app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "MyArea",
                    pattern: "{area:exists}/{controller=MyUser}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
