namespace MatchBoard.Web.Models.ViewModels
{
    public class SupervisorDashboardViewModel
    {
        public int TotalInterests { get; set; }
        public int OpenReviews { get; set; }
        public List<string> ExpertiseAreas { get; set; } = [];
        public List<SupervisorMatchViewModel> RecentMatches { get; set; } = [];
    }

    public class SupervisorMatchViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string ResearchArea { get; set; } = string.Empty;
        public DateTime MatchedAt { get; set; }
        public bool IsRevealed { get; set; }
    }
}
