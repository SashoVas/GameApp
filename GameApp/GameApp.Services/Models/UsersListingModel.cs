using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services.Models
{
    public class UsersListingModel
    {
        public string ImgUrl { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public bool IsFriend { get; set; }
        public int Games { get; set; }
    }
}
