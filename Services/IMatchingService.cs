using MatchBoard.Web.Models.ViewModels;

namespace MatchBoard.Web.Services
{
    public interface IMatchingService
    {
        Task<IReadOnlyList<BlindReviewViewModel>> GetBlindProjectsForSupervisorAsync(string supervisorId);
        Task<bool> ExpressInterestAsync(int projectId, string supervisorId);
    }
}
