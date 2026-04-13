using MatchBoard.Web.Data;
using MatchBoard.Web.Models.Domain;
using MatchBoard.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MatchBoard.Web.Services
{
    public class MatchingService : IMatchingService
    {
        private readonly ApplicationDbContext _context;

        public MatchingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<BlindReviewViewModel>> GetBlindProjectsForSupervisorAsync(string supervisorId)
        {
            var expertiseIds = await _context.SupervisorExpertises
                .Where(e => e.SupervisorId == supervisorId)
                .Select(e => e.ResearchAreaId)
                .ToListAsync();

            if (expertiseIds.Count == 0)
            {
                return [];
            }

            var interestedProjectIds = await _context.Matches
                .Where(m => m.SupervisorId == supervisorId)
                .Select(m => m.ProjectId)
                .ToListAsync();

            return await _context.Projects
                .AsNoTracking()
                .Include(p => p.ResearchArea)
                .Where(p => expertiseIds.Contains(p.ResearchAreaId) && p.Status == ProjectStatus.Pending)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new BlindReviewViewModel
                {
                    ProjectId = p.Id,
                    Title = p.Title,
                    Abstract = p.Abstract,
                    TechStack = p.TechnicalStack,
                    ResearchArea = p.ResearchArea!.Name,
                    SubmittedAt = p.CreatedAt,
                    HasExpressedInterest = interestedProjectIds.Contains(p.Id)
                })
                .ToListAsync();
        }

        public async Task<bool> ExpressInterestAsync(int projectId, string supervisorId)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.Status == ProjectStatus.Pending);

            if (project == null)
            {
                return false;
            }

            var existingMatch = await _context.Matches.AnyAsync(m => m.ProjectId == projectId);
            if (existingMatch)
            {
                return false;
            }

            _context.Matches.Add(new Match
            {
                ProjectId = projectId,
                SupervisorId = supervisorId,
                IsRevealed = false,
                MatchedAt = DateTime.UtcNow
            });

            project.Status = ProjectStatus.UnderReview;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
