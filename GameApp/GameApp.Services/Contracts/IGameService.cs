using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Services.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Task<bool> SetGameById(Comment game, int gameId);
        Task<bool> AddShoppingCartItem(ShoppingCart shoppingCart,int gameId);
        Task<bool> RemoveShoppingCartItem(ShoppingCart shoppingCart,int gameId);
        Task<PopularGamesServiceListingModel[]> GetUpcomingGames();
    }
}
