namespace MatchBoard.Web.Models.ViewModels
{
    public class BlindReviewViewModel
    {
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Abstract { get; set; } = string.Empty;
        public string TechStack { get; set; } = string.Empty;
        public string ResearchArea { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public bool HasExpressedInterest { get; set; }
    }
}
