
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
        }
        public ICollection<UserGame> Games { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public string? ImgURL { get; set; }
    }
}
