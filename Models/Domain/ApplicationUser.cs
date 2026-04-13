using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MatchBoard.Web.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        public ICollection<Project> Projects { get; set; } = [];
        public ICollection<SupervisorExpertise> Expertises { get; set; } = [];
        public ICollection<Match> SupervisedMatches { get; set; } = [];
    }
}
