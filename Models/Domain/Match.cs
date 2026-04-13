using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatchBoard.Web.Models.Domain
{
    public class Match
    {
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public string SupervisorId { get; set; } = string.Empty;

        public DateTime MatchedAt { get; set; } = DateTime.UtcNow;
        public bool IsRevealed { get; set; } = false;

        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }

        [ForeignKey(nameof(SupervisorId))]
        public ApplicationUser? Supervisor { get; set; }
    }
}
