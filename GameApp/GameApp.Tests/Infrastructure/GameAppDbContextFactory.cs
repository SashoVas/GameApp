using GameApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Tests.Infrastructure
{
    public static class GameAppDbContextFactory
    {

        public static GameAppDbContext InitializeContext()
        {
            var options = new DbContextOptionsBuilder<GameAppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            var context = new GameAppDbContext(options);
            return context;
        }

    }
}
