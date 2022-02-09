using GameApp.Services.Models;

namespace GameApp.Web.Areas.Profile.Models
{
    public class ProfileInfoViewModel
    {
        public string UserName { get; set; }
        public string ProfilePic { get; set; }
        public string Description { get; set; }
        public double MeanScore { get; set; }
        public int[] ScoreCounts { get; set; }
        public int Reviews { get; set; }
        public IEnumerable<GameInfoHelperModel> Games { get; set; }

    }
}
