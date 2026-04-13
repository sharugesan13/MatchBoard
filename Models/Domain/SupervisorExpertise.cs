using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatchBoard.Web.Models.Domain
{
    public class SupervisorExpertise
    {
        public int Id { get; set; }

        [Required]
        public string SupervisorId { get; set; } = string.Empty;

        [Required]
        public int ResearchAreaId { get; set; }

        [ForeignKey(nameof(SupervisorId))]
        public ApplicationUser? Supervisor { get; set; }

        [ForeignKey(nameof(ResearchAreaId))]
        public ResearchArea? ResearchArea { get; set; }
    }
}
