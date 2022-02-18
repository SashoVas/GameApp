using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services
{
    public class GenreService : IGenreService
    {
        private readonly IRepository<Genre> genres;
        public GenreService(IRepository<Genre> genres)
        {
            this.genres = genres;       
        }

        public async Task<bool> Create(string name)
        {
            await genres.AddAsync(new Genre { Name = name });
            await genres.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<string>> GetAll()
        {
            return await genres.All().Select(g=>g.Name).ToListAsync();
        }

        public async Task SetGenreToGameByName(Game game, IEnumerable<string> genreNames)
        {
               genreNames.ToList().ForEach( genre => game.Genres.Add(new GameGenre {Genre= genres.All().SingleOrDefault(g => g.Name == genre) }));
        }
    }
}
