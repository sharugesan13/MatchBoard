using System.ComponentModel.DataAnnotations;

namespace MatchBoard.Web.Models.Domain
{
    public class ResearchArea
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Tag { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<Project> Projects { get; set; } = [];
        public ICollection<SupervisorExpertise> SupervisorExpertises { get; set; } = [];
    }
}
