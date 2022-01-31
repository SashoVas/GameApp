namespace GameApp.Web.Areas.Profile.Models
{
    public class ProfileInfoViewModel
    {
        public string UserName { get; set; }
        public IEnumerable<string> Games { get; set; }
        public string ProfilePic { get; set; }
    }
}
