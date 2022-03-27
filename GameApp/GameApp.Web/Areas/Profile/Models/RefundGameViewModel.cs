using GameApp.Services.Models;

namespace GameApp.Web.Areas.Profile.Models
{
    public class RefundGameViewModel
    {
        public IEnumerable<RefundableItemsServiceModel> Games { get; set; }
    }
}
