using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services.Models
{
    public class UserInfoServiceModel
    {
        public string UserName { get; set; }
        public IEnumerable<string> Games { get; set; }
        public string ProfilePic { get; set; }
    }
}
