using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Services.Models;
using Microsoft.AspNetCore.Http;

namespace GameApp.Services.Contracts
{
    public interface IGameService
    {
        Task<GameServiceListingModel> GetGame(string name, string userId);
        Task<IEnumerable<AllGamesServiceListingModel>> GetAll(int page, string gameName, string genre, string username);
        Task<PopularGamesServiceListingModel[]> GetPopularGames();
        Task<PopularGamesServiceListingModel[]> GetTopRankedGames();
        Task<int> Create(string name,decimal price,string description, DateTime date, IEnumerable<string> newGenres, IFormFile image, string video);
        Task<bool> SetGameByName(Review review, string gameName);
        Task<bool> AddShoppingCartItem(ShoppingCart shoppingCart,int gameId);
        Task<PopularGamesServiceListingModel[]> GetUpcomingGames();
        Task<bool> IsUpcoming(int gameId);
        Task<bool> Deleate(string gameName);
        Task<bool> GameExist(int id);
        Task<bool> GameExistByName(string name);

    }
}
