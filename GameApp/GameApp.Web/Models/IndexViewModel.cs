using GameApp.Services.Models;

namespace GameApp.Web.Models
{
    public class IndexViewModel
    {
        public IEnumerable<GameServiceListingModel> Games { get; set; }
    }
}
