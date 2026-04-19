using MatchBoard.Web.Data;
using MatchBoard.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace MatchBoard.Web.Services
{
    public class MatchService
    {
        private readonly AppDbContext _context;

        public MatchService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Project>> GetAvailableProjectsAsync(string? researchArea)
        {
            var query = _context.Projects
                .Where(p => p.Status == "Pending" || p.Status == "UnderReview");

            if (!string.IsNullOrEmpty(researchArea))
                query = query.Where(p => p.ResearchArea == researchArea);

            return await query.ToListAsync();
        }

        public async Task<bool> ConfirmMatchAsync(int projectId, string supervisorId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null || project.SupervisorId != supervisorId)
                return false;

            project.Status = "Matched";
            project.IdentityRevealed = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExpressInterestAsync(int projectId, string supervisorId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null) return false;

            project.SupervisorId = supervisorId;
            project.Status = "UnderReview";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(Project? project, string? studentName, string? studentEmail)> GetRevealedMatchAsync(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null || !project.IdentityRevealed)
                return (null, null, null);

            return (project, project.StudentId, null);
        }
    }
}