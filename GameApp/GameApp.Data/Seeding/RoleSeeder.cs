using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Data.Seeding
{
    public class RoleSeeder : ISeeder
    {
        public async Task Seed(GameAppDbContext context)
        {
            if (!context.Roles.Any())
            {
                await context.Roles.AddAsync(new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" });
                await context.Roles.AddAsync(new IdentityRole { Name = "User", NormalizedName = "USER" });
            }
        }
    }
}
