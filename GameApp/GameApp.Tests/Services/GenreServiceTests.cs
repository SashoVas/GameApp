using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services;
using GameApp.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GameApp.Tests.Services
{
    public class GenreServiceTests
    {
        private List<Genre>GetDummyData()
        {
            var genres=new List<Genre>();
            for (int i = 1; i < 16; i++)
            {
                var genre = new Genre
                {
                    Id = i,
                    Name="Genre"+i.ToString(),
                };
                genres.Add(genre);
            }
            return genres;
        }
        private async Task SeedData(GameAppDbContext context)
        {
            context.AddRange(GetDummyData());
            await context.SaveChangesAsync();
        }
        [Fact]
        public async Task TestGetAllShouldReturnAllGenres()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Genre>(context);
            var genreService = new GenreService(repo);

            var result=(await genreService
                .GetAll())
                .ToList();

            var actual = repo
                .All()
                .ToList();

            for (int i = 0; i < result.Count(); i++)
            {
                Assert.Equal(result[i],actual[i].Name);
            }
        }
        [Fact]
        public async Task TestCreateGenreShouldCreateGenre()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Genre>(context);
            var genreService = new GenreService(repo);

            await genreService.Create("NewGenre");

            var result = repo
                .All()
                .Last();
            Assert.Equal(result.Name, "NewGenre");
        }

        [Fact]
        public async Task TestCreateGenreWithImproperValueShouldThrowExeption()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Genre>(context);
            var genreService = new GenreService(repo);
            Assert.False(await genreService.Create(null));
        }

    }
}
