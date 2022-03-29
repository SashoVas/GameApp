using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using GameApp.Services.Models;
using Microsoft.AspNetCore.Identity;
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
        private readonly IGameService gameService;
        private readonly IRepository<UserGame> userGames;
        private readonly UserManager<User> userManager;
        private readonly IReceiptService receiptService;
        public CartService(ShoppingCart shoppingCart, IRepository<UserGame> userGames, UserManager<User> userManager, IGameService gameService, IReceiptService receiptService)
        {
            this.shoppingCart = shoppingCart;
            this.userManager = userManager;
            this.userGames = userGames;
            this.gameService = gameService;
            this.receiptService = receiptService;
        }
        public async Task<bool> BuyItems(string userId,string cardId)
        {
            var cartItems = await shoppingCart
                .GetCartItems()
                .ToListAsync();

            var user = await userManager
                .FindByIdAsync(userId);
            if (user==null)
            {
                return false;
            }
            var gamesForReceipt = new List<UserGame>();
            cartItems
                .ForEach(async item =>
                {
                    var ug = new UserGame
                    {
                        Game = item,
                        User = user
                    };
                    await userGames.AddAsync(ug);
                    gamesForReceipt.Add(ug); 

                });
            var success=await receiptService.CreateReceipt(userId, gamesForReceipt, cardId,ReceiptType.Purchase);
            if (!success)
            {
                return false;
            }

            await userGames.SaveChangesAsync();
            await shoppingCart.Clear();


            return true;
        }
        public async Task<bool> AddToCart(int id)
        {
            var success=await gameService.AddShoppingCartItem(shoppingCart, id);
            return success;
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

            var success=await gameService.RemoveShoppingCartItem(shoppingCart, id);
            return success;
           
        }
    }
}
