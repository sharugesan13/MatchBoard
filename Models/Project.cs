using System.ComponentModel.DataAnnotations;

namespace MatchBoard.Web.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Abstract { get; set; } = string.Empty;

        [Required]
        public string TechStack { get; set; } = string.Empty;

        [Required]
        public string ResearchArea { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public string StudentId { get; set; } = string.Empty;

        public string? SupervisorId { get; set; }

        public bool IdentityRevealed { get; set; } = false;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
