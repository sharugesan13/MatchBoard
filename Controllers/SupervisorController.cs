using MatchBoard.Web.Data;
using MatchBoard.Web.Models.Domain;
using MatchBoard.Web.Models.ViewModels;
using MatchBoard.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MatchBoard.Web.Controllers
{
    [Authorize(Roles = "Supervisor")]
    public class SupervisorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMatchingService _matchingService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SupervisorController(
            ApplicationDbContext context,
            IMatchingService matchingService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _matchingService = matchingService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var supervisorId = _userManager.GetUserId(User)!;

            var recentMatches = await _context.Matches
                .AsNoTracking()
                .Include(m => m.Project)
                    .ThenInclude(p => p!.ResearchArea)
                .Where(m => m.SupervisorId == supervisorId)
                .OrderByDescending(m => m.MatchedAt)
                .Select(m => new SupervisorMatchViewModel
                {
                    ProjectId = m.ProjectId,
                    ProjectTitle = m.Project!.Title,
                    ResearchArea = m.Project.ResearchArea!.Name,
                    MatchedAt = m.MatchedAt,
                    IsRevealed = m.IsRevealed
                })
                .Take(5)
                .ToListAsync();

            var expertiseAreas = await _context.SupervisorExpertises
                .AsNoTracking()
                .Include(e => e.ResearchArea)
                .Where(e => e.SupervisorId == supervisorId)
                .Select(e => e.ResearchArea!.Name)
                .OrderBy(name => name)
                .ToListAsync();

            var model = new SupervisorDashboardViewModel
            {
                TotalInterests = await _context.Matches.CountAsync(m => m.SupervisorId == supervisorId),
                OpenReviews = await _context.Matches.CountAsync(m => m.SupervisorId == supervisorId && !m.IsRevealed),
                ExpertiseAreas = expertiseAreas,
                RecentMatches = recentMatches
            };

            return View(model);
        }

        public async Task<IActionResult> Browse()
        {
            var supervisorId = _userManager.GetUserId(User)!;
            var projects = await _matchingService.GetBlindProjectsForSupervisorAsync(supervisorId);

            var hasExpertise = await _context.SupervisorExpertises
                .AnyAsync(e => e.SupervisorId == supervisorId);

            if (!hasExpertise)
            {
                TempData["Error"] = "Set your expertise areas first to start the blind review process.";
            }

            return View(projects);
        }

        public async Task<IActionResult> SetExpertise()
        {
            var supervisorId = _userManager.GetUserId(User)!;
            var allAreas = await _context.ResearchAreas
                .AsNoTracking()
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();

            var selectedIds = await _context.SupervisorExpertises
                .AsNoTracking()
                .Where(e => e.SupervisorId == supervisorId)
                .Select(e => e.ResearchAreaId)
                .ToListAsync();

            var model = new SupervisorExpertiseViewModel
            {
                SelectedAreaIds = selectedIds,
                Areas = allAreas.Select(a => new ResearchAreaSelectionViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Tag = a.Tag,
                    IsSelected = selectedIds.Contains(a.Id)
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetExpertise(SupervisorExpertiseViewModel model)
        {
            var supervisorId = _userManager.GetUserId(User)!;

            if (model.SelectedAreaIds.Count == 0)
            {
                var areas = await _context.ResearchAreas
                    .AsNoTracking()
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.Name)
                    .ToListAsync();

                model.Areas = areas.Select(a => new ResearchAreaSelectionViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Tag = a.Tag,
                    IsSelected = false
                }).ToList();

                ModelState.AddModelError(nameof(model.SelectedAreaIds), "Select at least one research area.");
                return View(model);
            }

            var existingExpertise = await _context.SupervisorExpertises
                .Where(e => e.SupervisorId == supervisorId)
                .ToListAsync();

            _context.SupervisorExpertises.RemoveRange(existingExpertise);

            foreach (var areaId in model.SelectedAreaIds.Distinct())
            {
                _context.SupervisorExpertises.Add(new SupervisorExpertise
                {
                    SupervisorId = supervisorId,
                    ResearchAreaId = areaId
                });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Research expertise updated successfully.";
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExpressInterest(int projectId)
        {
            var supervisorId = _userManager.GetUserId(User)!;
            var success = await _matchingService.ExpressInterestAsync(projectId, supervisorId);

            TempData[success ? "Success" : "Error"] = success
                ? "Interest expressed. The project moved to Under Review."
                : "Could not express interest. The project may already be under review or matched.";

            return RedirectToAction(nameof(MyMatches));
        }

        public async Task<IActionResult> MyMatches()
        {
            var supervisorId = _userManager.GetUserId(User)!;
            var matches = await _context.Matches
                .AsNoTracking()
                .Include(m => m.Project)
                    .ThenInclude(p => p!.ResearchArea)
                .Where(m => m.SupervisorId == supervisorId)
                .OrderByDescending(m => m.MatchedAt)
                .Select(m => new SupervisorMatchViewModel
                {
                    ProjectId = m.ProjectId,
                    ProjectTitle = m.Project!.Title,
                    ResearchArea = m.Project.ResearchArea!.Name,
                    MatchedAt = m.MatchedAt,
                    IsRevealed = m.IsRevealed
                })
                .ToListAsync();

            return View(matches);
        }
    }
}
