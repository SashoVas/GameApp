using System.ComponentModel.DataAnnotations;

namespace GameApp.Data.Models
{
    public class Comment
    {
        public Comment()
        {
            this.Comments=new HashSet<Comment>();
        }
        public string Id { get; set; }
        [Required]
        [MaxLength(300)]
        public string Content { get; set; }
        [Required]
        public DateTime PostedOn { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public int GameId { get; set; }
        [Required]
        public Game Game { get; set; }
        public string? CommentedOnId { get; set; }
        public Comment? CommentedOn { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
