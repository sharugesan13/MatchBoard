using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchBoard.Web.Data;
using MatchBoard.Web.Models;

namespace MatchBoard.Web.Controllers
{
    [Authorize(Roles = "Supervisor")]
    public class SupervisorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SupervisorController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Blind Review Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var supervisor = await _userManager.GetUserAsync(User);
            var projects = await _context.Projects
                .Where(p => p.Status == "Pending" || p.Status == "UnderReview")
                .Where(p => supervisor!.ResearchArea == null || p.ResearchArea == supervisor.ResearchArea)
                .Select(p => new Project
                {
                    Id = p.Id,
                    Title = p.Title,
                    Abstract = p.Abstract,
                    TechStack = p.TechStack,
                    ResearchArea = p.ResearchArea,
                    Status = p.Status,
                    // StudentId intentionally NOT passed - blind matching
                })
                .ToListAsync();

            return View(projects);
        }

        // Express interest in a project
        [HttpPost]
        public async Task<IActionResult> ExpressInterest(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null) return NotFound();

            project.SupervisorId = _userManager.GetUserId(User);
            project.Status = "UnderReview";
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }

        // Confirm match and reveal identities
        [HttpPost]
        public async Task<IActionResult> ConfirmMatch(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null) return NotFound();

            var supervisorId = _userManager.GetUserId(User);
            if (project.SupervisorId != supervisorId) return Forbid();

            project.Status = "Matched";
            project.IdentityRevealed = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("MyMatches");
        }

        // View matched projects with revealed identities
        public async Task<IActionResult> MyMatches()
        {
            var supervisorId = _userManager.GetUserId(User);
            var matches = await _context.Projects
                .Where(p => p.SupervisorId == supervisorId && p.Status == "Matched")
                .ToListAsync();

            var matchDetails = new List<(Project project, ApplicationUser? student)>();
            foreach (var project in matches)
            {
                var student = await _userManager.FindByIdAsync(project.StudentId);
                matchDetails.Add((project, student));
            }

            ViewBag.MatchDetails = matchDetails;
            return View(matches);
        }

        // Expertise management
        public async Task<IActionResult> SetExpertise()
        {
            var supervisor = await _userManager.GetUserAsync(User);
            ViewBag.CurrentArea = supervisor?.ResearchArea;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SetExpertise(string researchArea)
        {
            var supervisor = await _userManager.GetUserAsync(User);
            if (supervisor != null)
            {
                supervisor.ResearchArea = researchArea;
                await _userManager.UpdateAsync(supervisor);
            }
            return RedirectToAction("Dashboard");
        }
    }
}