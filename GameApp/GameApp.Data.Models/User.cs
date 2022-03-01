
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string? Description { get; set; }
        public string? ImgURL { get; set; }
    }
}
