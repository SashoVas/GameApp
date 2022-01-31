using GameApp.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Data
{
    public class GameAppDbContext:IdentityDbContext<User,IdentityRole,string>
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<UserGame> UserGames { get; set; }
        public DbSet<ShoppingCartGame> ShoppingCartGames { get; set; }
        public GameAppDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserGame>()
                .HasKey(ug => new { ug.UserId, ug.GameId })
                .HasName("PrimaryKey_UserGamesId");

            builder.Entity<Game>()
                .HasMany(g => g.Users)
                .WithOne(ug => ug.Game)
                .HasForeignKey(ug => ug.GameId);

            builder.Entity<User>()
                .HasMany(u => u.Games)
                .WithOne(ug => ug.User)
                .HasForeignKey(ug => ug.UserId);

            builder.Entity<ShoppingCartGame>()
                .HasOne(sc => sc.Game)
                .WithMany(g => g.ShoppingCartGames)
                .HasForeignKey(sc => sc.GameId);
        }

    }
}
