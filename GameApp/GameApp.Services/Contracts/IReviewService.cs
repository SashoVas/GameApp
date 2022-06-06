namespace GameApp.Services.Contracts
{
    public interface IReviewService
    {
        Task<bool> Rate(string gameName, int points, string userId);
    }
}
