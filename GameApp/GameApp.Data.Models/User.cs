using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GameApp.Data.Models
{
    public class User:IdentityUser
    {
        public User()
        {
            Games=new HashSet<UserGame>();
            Comments = new HashSet<Comment>();
            Reviews = new HashSet<Review>();
            Friends=new HashSet<Friend>();
            Cards = new HashSet<Card>();
        }
        public ICollection<UserGame> Games { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Friend> Friends { get; set; }
        public ICollection<Card> Cards { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        [MaxLength(100)]
        public string? ImgURL { get; set; }
    }
}
