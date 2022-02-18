using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Data.Seeding
{
    internal interface ISeeder
    {
        Task Seed(GameAppDbContext context);
    }
}
