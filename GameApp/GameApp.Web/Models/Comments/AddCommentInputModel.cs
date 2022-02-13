using System.ComponentModel.DataAnnotations;

namespace GameApp.Web.Models.Comments
{
    public class AddCommentInputModel
    {
        [Required]
        public int GameId { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(300)]
        public string Contents { get; set; }
    }
}
