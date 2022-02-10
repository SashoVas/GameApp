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
        Task<int> Create(string name,decimal price,string description, IEnumerable<string> newGenres, IFormFile image);
        Task<bool> BuyItems(string userId);
        Task<bool> Rate(string gameName,int points,string userId);
    }
}
