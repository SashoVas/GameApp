using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using GameApp.Services.Models;
using Microsoft.EntityFrameworkCore;


namespace GameApp.Services
{
    public class CartService : ICartService
    {
        private readonly ShoppingCart shoppingCart;
        private readonly IGameService gameService;
        private readonly IRepository<UserGame> userGames;
        private readonly IReceiptService receiptService;
        public CartService(ShoppingCart shoppingCart, IRepository<UserGame> userGames, IGameService gameService, IReceiptService receiptService)
        {
            this.shoppingCart = shoppingCart;
            this.userGames = userGames;
            this.gameService = gameService;
            this.receiptService = receiptService;
        }
        public async Task<bool> BuyItems(string userId,string cardId)
        {
            var cartItems = await shoppingCart
                .GetCartItems()
                .ToListAsync();
            var gamesForReceipt = new List<UserGame>();
            cartItems
                .ForEach(async item =>
                {
                    var ug = new UserGame
                    {
                        Game = item,
                        UserId = userId
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
            => await gameService.AddShoppingCartItem(shoppingCart, id);

        public async Task<bool> Clear() 
            => await shoppingCart.Clear();

        public async Task<IEnumerable<CartServiceListingModel>> GetAllItems() 
            => await shoppingCart.GetCartItems().Select(x => new CartServiceListingModel
            {
                Id = x.Id,
                GameName = x.Name,
                Price = x.Price
            }).ToListAsync();

        public async Task<bool> RemoveFromCart(int id) 
            => await shoppingCart.RemoveFromCart(id);
    }
}
