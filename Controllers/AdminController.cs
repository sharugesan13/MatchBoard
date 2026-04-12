
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchBoard.Web.Data;
using MatchBoard.Web.Models;

namespace MatchBoard.Web.Controllers
{
    [Authorize(Roles = "ModuleLeader")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Overview dashboard
        public async Task<IActionResult> Dashboard()
        {
            var projects = await _context.Projects.ToListAsync();
            var users = _userManager.Users.ToList();
            ViewBag.TotalProjects = projects.Count;
            ViewBag.MatchedProjects = projects.Count(p => p.Status == "Matched");
            ViewBag.PendingProjects = projects.Count(p => p.Status == "Pending");
            ViewBag.TotalUsers = users.Count;
            return View(projects);
        }

        // Reassign a project to a different supervisor
        [HttpPost]
        public async Task<IActionResult> Reassign(int projectId, string newSupervisorId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null) return NotFound();

            project.SupervisorId = newSupervisorId;
            project.Status = "UnderReview";
            project.IdentityRevealed = false;
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }

        // User management
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // Delete user
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
                await _userManager.DeleteAsync(user);
            return RedirectToAction("Users");
        }

        // Research area management
        public IActionResult ResearchAreas()
        {
            var areas = new List<string>
            {
                "Artificial Intelligence",
                "Web Development",
                "Cybersecurity",
                "Cloud Computing",
                "Machine Learning",
                "Mobile Development",
                "Internet of Things"
            };
            return View(areas);
        }
    }
}