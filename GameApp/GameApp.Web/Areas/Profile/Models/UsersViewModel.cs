using GameApp.Services.Models;

namespace GameApp.Web.Areas.Profile.Models
{
    public class UsersViewModel
    {
        public IEnumerable<UsersListingModel> Users { get; set; }
        public int Page { get; set; }
        public string Username { get; set; }
    }
}
