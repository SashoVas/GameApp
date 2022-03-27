using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services.Contracts;
using GameApp.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services
{
    public class UserGameService : IUserGameService
    {
        private readonly IRepository<UserGame> userGames;

        public UserGameService(IRepository<UserGame> userGames)
        {
            this.userGames = userGames;
        }

        public async Task<IEnumerable<RefundableItemsServiceModel>> GetGameForRefund(string userId)
        {
            var games = userGames
                .All()
                .Include(ug=>ug.Game)
                .Where(ug => ug.UserId == userId && ug.Date > DateTime.Now.AddDays(-3));
            return await games.Select(g => new RefundableItemsServiceModel
            {
                IMG = g.Game.ImageUrl,
                Name = g.Game.Name,
                ReleaseDate = g.Game.ReleaseDate,
                GameId=g.Game.Id
            }).ToListAsync();
        }

        public async Task<bool> RefundGame(int gameId, string userId)
        {
            var userGame =await userGames.All().FirstOrDefaultAsync(ug=>ug.GameId==gameId&&ug.UserId==userId);

            if (userGame == null)
            {
                return false;
            }
            userGames.Delete(userGame);
            await userGames.SaveChangesAsync();
            return true;

        }
    }
}
