using System.ComponentModel.DataAnnotations;

namespace GameApp.Web.Models.Friend
{
    public class FriendRequestInputModel 
    {
        [Required]
        public string Username { get; set; }
    }
}
