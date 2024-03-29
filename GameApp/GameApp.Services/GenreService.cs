﻿using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GameApp.Services
{
    public class GenreService : IGenreService
    {
        private readonly IRepository<Genre> genres;
        public GenreService(IRepository<Genre> genres) 
            => this.genres = genres;

        public async Task<bool> Create(string name)
        {
            if (name==null)
            {
                return false;
            }
            await genres.AddAsync(new Genre { Name = name });
            await genres.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<string>> GetAll() 
            => await genres.All()
            .Select(g => g.Name)
            .ToListAsync();

        public async Task SetGenreToGameByName(Game game, IEnumerable<string> genreNames) 
            => genreNames.ToList()
            .ForEach(genre => game.Genres
                .Add(new GameGenre 
                { 
                    Genre = genres.All().SingleOrDefault(g => g.Name == genre) 
                }));
    }
}
