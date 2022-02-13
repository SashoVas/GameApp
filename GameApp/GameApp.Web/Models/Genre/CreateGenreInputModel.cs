using System.ComponentModel.DataAnnotations;

namespace GameApp.Web.Models.Genre
{
    public class CreateGenreInputModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Name { get; set; }
    }
}
