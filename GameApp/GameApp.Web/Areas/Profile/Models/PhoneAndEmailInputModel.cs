using System.ComponentModel.DataAnnotations;

namespace GameApp.Web.Areas.Profile.Models
{
    public class PhoneAndEmailInputModel
    {
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "InvalidEmail")]
        public string? Email { get; set; }
        [RegularExpression("^([0-9]{10})$", ErrorMessage = "Your phone number should be 10 numbers between 0-9")]
        public string? PhoneNumber { get; set; }
    }
}
