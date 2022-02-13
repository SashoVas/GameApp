using System.ComponentModel.DataAnnotations;

namespace GameApp.Web.Models.Game
{
    public class GameRateInputModel
    {
        [Required]
        [Range(1,10)]
        public int Points { get; set; }
        [Required]
        public string GameName { get; set; }
    }
}
