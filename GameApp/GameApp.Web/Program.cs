
using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services;
using GameApp.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<GameAppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

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

builder.Services.AddControllersWithViews(configure=>configure.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
builder.Services.AddScoped(typeof(IRepository<>),typeof( Repository<>));
builder.Services.AddTransient<IGameService, GameService>();
builder.Services.AddTransient<ICartService, CartService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IGenreService, GenreService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped(sp => ShoppingCart.GetShoppingCart(sp));
//builder.Services.AddScoped<IRepository<>,Repository<>>;



builder.Services.AddSession();
var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())//app.ApplicationServices.CreateScope())
{
    using (var context = serviceScope.ServiceProvider.GetRequiredService<GameAppDbContext>())
    {
        //context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        if (!context.Roles.Any())
        {
            context.Roles.Add(new IdentityRole { Name = "admin", NormalizedName = "ADMIN" });
            context.Roles.Add(new IdentityRole { Name = "user", NormalizedName = "USER" });
        }
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "MyArea",
        pattern: "{area:exists}/{controller=MyUser}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});


app.MapRazorPages();

app.Run();
