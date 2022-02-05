using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services.Models
{
    public class CommentsServiceListingModel
    {
        public string Username { get; set; }
        public string Contents { get; set; }
        public string PostedOn { get; set; }
        public int Rating { get; set; }
        public string CommentId { get; set; }
    }
}
