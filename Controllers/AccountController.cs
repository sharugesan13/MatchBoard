using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MatchBoard.Web.Models;

namespace MatchBoard.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string password)
        {
            var user = new ApplicationUser
            {
                FullName = fullName,
                UserName = email,
                Email = email,
                Role = "Student"
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Student");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View();
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    

public IActionResult ModuleLeaderRegister() => View();

[HttpPost]
public async Task<IActionResult> ModuleLeaderRegister(string fullName, string email, string password)
{
    var user = new ApplicationUser
    {
        FullName = fullName,
        UserName = email,
        Email = email,
        Role = "ModuleLeader"
    };

    var result = await _userManager.CreateAsync(user, password);

    if (result.Succeeded)
    {
        await _userManager.AddToRoleAsync(user, "ModuleLeader");
        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Dashboard", "Admin");
    }

    foreach (var error in result.Errors)
        ModelState.AddModelError("", error.Description);

    return View();
}
}
}
