using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime PostedOn { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
        public int? CommentedOnId { get; set; }
        public Comment CommentedOn { get; set; }
        public IEnumerable<Comment> Comments { get; set; }



    }
}
