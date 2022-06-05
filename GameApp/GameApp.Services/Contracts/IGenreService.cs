using GameApp.Data.Models;

namespace GameApp.Services.Contracts
{
    public interface IGenreService
    {
        Task<IEnumerable<string>> GetAll();
        Task<bool> Create(string name);
        Task SetGenreToGameByName(Game game, IEnumerable<string> genreNames);
    }
}
