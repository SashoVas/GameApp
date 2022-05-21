using GameApp.Data.Models;
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
        public string ProfilePic { get; set; }
        public string Description { get; set; }
        public int ReviewsCount { get; set; }
        public double MeanScore { get; set; }
        public int[] ScoreCount { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        public IEnumerable<GameInfoHelperModel> Games { get; set; }
    }
}
