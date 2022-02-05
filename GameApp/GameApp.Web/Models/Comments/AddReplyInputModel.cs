namespace GameApp.Web.Models.Comments
{
    public class AddReplyInputModel
    {
        public int GameId { get; set; }
        public string Contents { get; set; }
        public string CommentId { get; set; }
    }
}
