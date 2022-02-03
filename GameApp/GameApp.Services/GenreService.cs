﻿using GameApp.Data.Models;
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
        public async Task<IEnumerable<string>> GetAll()
        {
            return await genres.All().Select(g=>g.Name).ToListAsync();
        }
    }
}