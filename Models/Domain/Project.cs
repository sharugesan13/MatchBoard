using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatchBoard.Web.Models.Domain
{
    public enum ProjectStatus
    {
        Pending = 0,
        UnderReview = 1,
        Matched = 2,
        Withdrawn = 3
    }

    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 10)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000, MinimumLength = 50)]
        public string Abstract { get; set; } = string.Empty;

        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string TechnicalStack { get; set; } = string.Empty;

        public ProjectStatus Status { get; set; } = ProjectStatus.Pending;

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [ForeignKey(nameof(StudentId))]
        public ApplicationUser? Student { get; set; }

        [Required]
        public int ResearchAreaId { get; set; }

        [ForeignKey(nameof(ResearchAreaId))]
        public ResearchArea? ResearchArea { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
