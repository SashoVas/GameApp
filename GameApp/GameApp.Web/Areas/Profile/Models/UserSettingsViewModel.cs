using GameApp.Services.Models;

namespace GameApp.Web.Areas.Profile.Models
{
    public class UserSettingsViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public IEnumerable<AllCardsServiceListingModel> Cards { get; set; }
        public IEnumerable<GameInfoHelperModel> Games { get; set; }

    }
}
