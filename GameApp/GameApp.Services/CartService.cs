using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using GameApp.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services
{
    public class CartService : ICartService
    {
        private readonly ShoppingCart shoppingCart;
        private readonly IRepository<Game> games;
        public CartService(ShoppingCart shoppingCart, IRepository<Game> games)
        {
            this.shoppingCart = shoppingCart;
            this.games = games;
        }
        public async Task<bool> AddToCart(int id)
        {
            var game = games.All().SingleOrDefault(g => g.Id == id);
            if (game==null)
            {
                return false;
            }
            await shoppingCart.AddToCart(game);
            return true;
        }

        public async Task<bool> Clear()
        {
            await shoppingCart.Clear();
            return true;
        }

        public async Task<IEnumerable<CartServiceListingModel>> GetAllItems()
        {
            var value=await shoppingCart.GetCartItems().Select(x => new CartServiceListingModel
            {
                Id=x.Id,
                GameName=x.Name,
                Price=x.Price
            }).ToListAsync();
            return value;
        }

        public async Task<bool> RemoveFromCart(int id)
        {
            var item=games.All().SingleOrDefault(game => game.Id == id);
            if (item==null)
            {
                return false;
            }
            if (await shoppingCart.RemoveFromCart(item))
            {
                return true;
            }
            return false;
        }
    }
}
