using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Data.Models
{
    public class Friend
    {
        public int Id { get; set; }
        public string MainUserId { get; set; }
        public User MainUser { get; set; }
        public string FriendUserId { get; set; }
        public User FriendUser { get; set; }
        public string Status { get; set; }

    }
}
