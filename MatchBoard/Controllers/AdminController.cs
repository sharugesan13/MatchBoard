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

        // =========================
        // DASHBOARD
        // =========================
        public async Task<IActionResult> Dashboard()
        {
            var projects = await _context.Projects.ToListAsync();
            var users = await _userManager.Users.ToListAsync();

            ViewBag.TotalProjects = projects.Count;
            ViewBag.MatchedProjects = projects.Count(p => p.Status == "Matched");
            ViewBag.PendingProjects = projects.Count(p => p.Status == "Pending");
            ViewBag.TotalUsers = users.Count;

            return View(projects);
        }

        // =========================
        // REASSIGN SUPERVISOR (SAFE)
        // =========================
        [HttpPost]
        public async Task<IActionResult> Reassign(int projectId, string newSupervisorId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null) return NotFound();

            var supervisor = await _userManager.FindByIdAsync(newSupervisorId);
            if (supervisor == null)
                return BadRequest("Invalid supervisor ID");

            var roles = await _userManager.GetRolesAsync(supervisor);
            if (!roles.Contains("Supervisor"))
                return BadRequest("Selected user is not a supervisor");

            project.SupervisorId = newSupervisorId;
            project.Status = "UnderReview";
            project.IdentityRevealed = false;

            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }

        // =========================
        // FINAL MATCH APPROVAL
        // =========================
        public async Task<IActionResult> Approve(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            project.Status = "Matched";

            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard");
        }

        // =========================
        // REJECT PROJECT
        // =========================
        public async Task<IActionResult> Reject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            project.Status = "Rejected";
            project.SupervisorId = null;

            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard");
        }

        // =========================
        // DELETE PROJECT
        // =========================
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Dashboard");
        }

        // =========================
        // USERS
        // =========================
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("Users");
        }

        // =========================
        // RESEARCH AREAS
        // =========================
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