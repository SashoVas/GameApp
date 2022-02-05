using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services.Models
{
    public class ReplyServiceListingModel
    {
        public string Username { get; set; }
        public string Content { get; set; }
        public string CommentId { get; set; }
        public bool HasComments { get; set; }
    }
}
