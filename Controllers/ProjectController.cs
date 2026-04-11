using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchBoard.Web.Data;
using MatchBoard.Web.Models;

namespace MatchBoard.Web.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var projects = await _context.Projects
                .Where(p => p.StudentId == userId)
                .ToListAsync();
            return View(projects);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Project project)
        {
            if (ModelState.IsValid)
            {
                project.StudentId = _userManager.GetUserId(User)!;
                project.Status = "Pending";
                project.SubmittedAt = DateTime.UtcNow;
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.StudentId == userId);
            if (project == null) return NotFound();
            return View(project);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Projects.Update(project);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        [HttpPost]
        public async Task<IActionResult> Withdraw(int id)
        {
            var userId = _userManager.GetUserId(User);
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.StudentId == userId && p.Status == "Pending");
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}