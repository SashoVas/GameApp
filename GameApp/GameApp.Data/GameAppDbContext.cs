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
        public DbSet<Genre> Genres { get; set; }
        public DbSet<GameGenre> GameGenres { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<Friend> Friends { get; set; }

        public GameAppDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserGame>()
                .HasKey(ug => new { ug.UserId, ug.GameId })
                .HasName("PrimaryKey_UserGamesId");

            builder.Entity<Review>()
                .HasKey(r => new { r.UserId, r.GameId })
                .HasName("PrimaryKey_ReviewId");

            builder.Entity<GameGenre>()
                .HasKey(gg => new { gg.GenreId, gg.GameId })
                .HasName("PrimaryKey_GameGenresId");

            builder.Entity<Friend>()
            .HasKey(f => new { f.MainUserId, f.FriendUserId })
            .HasName("PrimaryKey_UserFriendId");

            builder.Entity<Game>()
                .HasMany(g => g.Users)
                .WithOne(ug => ug.Game)
                .HasForeignKey(ug => ug.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<User>()
                .HasMany(u => u.Games)
                .WithOne(ug => ug.User)
                .HasForeignKey(ug => ug.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ShoppingCartGame>()
                .HasOne(sc => sc.Game)
                .WithMany(g => g.ShoppingCartGames)
                .HasForeignKey(sc => sc.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasMany(c => c.Comments)
                .WithOne(c => c.CommentedOn)
                .HasForeignKey(c => c.CommentedOnId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Receipt>()
                .HasMany(r => r.UserGames)
                .WithOne(ug => ug.Receipt)
                .HasForeignKey(wg => wg.ReceiptId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Friend>()
                .HasOne(f => f.MainUser)
                .WithMany(u => u.Friends)
                .HasForeignKey(f => f.MainUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Friend>()
                .HasOne(f => f.FriendUser)
                .WithMany()
                .HasForeignKey(f => f.FriendUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
