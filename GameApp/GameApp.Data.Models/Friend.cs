using System.ComponentModel.DataAnnotations;

namespace GameApp.Data.Models
{
    public class Friend
    {
        public int Id { get; set; }
        [Required]
        public string MainUserId { get; set; }
        [Required]
        public User MainUser { get; set; }
        [Required]
        public string FriendUserId { get; set; }
        [Required]
        public User FriendUser { get; set; }
        [Required]
        public FriendStatus Status { get; set; }

    }
}
