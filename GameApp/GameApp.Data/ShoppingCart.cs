using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GameApp.Data
{
    public class ShoppingCart
    {
        public string Id { get; set; }
        private readonly IRepository<ShoppingCartGame> shoppingCartGames;
        public ShoppingCart(IRepository<ShoppingCartGame> shoppingCartGames)
        {
            this.shoppingCartGames = shoppingCartGames;
        }
        public static ShoppingCart GetShoppingCart(IServiceProvider serviceProvider)
        {
            var sesions = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext.Session;
            var shoppingCartGames = serviceProvider.GetRequiredService<IRepository<ShoppingCartGame>>();
            string id = sesions.GetString("CartId");
            if (id==null)
            {
                id= Guid.NewGuid().ToString();
            }
            sesions.SetString("CartId", id);
            return new ShoppingCart(shoppingCartGames) { Id=id};
        }
        public async Task<bool> AddToCart(Game game) 
        {
            var item = shoppingCartGames
                .All()
                .SingleOrDefault(item => item.ShoppingCartId == this.Id 
                && item.GameId == game.Id);
            if (item==null)
            {
                await shoppingCartGames
                    .AddAsync(new ShoppingCartGame 
                    {
                        Game=game,
                        ShoppingCartId=this.Id ,
                        GameId=game.Id,
                        Id=Guid.NewGuid().ToString()
                    });
                await shoppingCartGames.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> RemoveFromCart(int gameId)
        {
            var item = shoppingCartGames
                .All()
                .SingleOrDefault(item => item.ShoppingCartId == this.Id
                && item.GameId == gameId);
            if (item==null)
            {
                return false;
            }
            shoppingCartGames.Delete(item);
            await shoppingCartGames.SaveChangesAsync();

            return true;
        }
        public IQueryable<Game>GetCartItems()
        {
            var items = shoppingCartGames
                .All()
                .Where(item => item.ShoppingCartId == this.Id)
                .Select(item => item.Game);
            return items;
        }
        public async Task<bool>Clear()
        {
            var items =await shoppingCartGames
                .All()
                .Where(item => item.ShoppingCartId == this.Id).ToListAsync();
            shoppingCartGames.DeleteRange(items);
            await shoppingCartGames.SaveChangesAsync();
            return true;
        }

    }
}
