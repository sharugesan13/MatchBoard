using System.ComponentModel.DataAnnotations;

namespace MatchBoard.Web.Models.ViewModels
{
    public class SupervisorExpertiseViewModel
    {
        [MinLength(1, ErrorMessage = "Select at least one research area.")]
        public List<int> SelectedAreaIds { get; set; } = [];

        public List<ResearchAreaSelectionViewModel> Areas { get; set; } = [];
    }

    public class ResearchAreaSelectionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}
