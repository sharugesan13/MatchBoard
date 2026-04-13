using MatchBoard.Web.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MatchBoard.Web.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            string[] roles = ["Supervisor", "Student", "Admin"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var supervisor = await EnsureUserAsync(
                userManager,
                "supervisor@matchboard.local",
                "Supervisor@123",
                "Dr. Maya Perera",
                "Supervisor");

            var studentOne = await EnsureUserAsync(
                userManager,
                "student1@matchboard.local",
                "Student@123",
                "Nimal Perera",
                "Student");

            var studentTwo = await EnsureUserAsync(
                userManager,
                "student2@matchboard.local",
                "Student@123",
                "Kavya Fernando",
                "Student");

            var admin = await EnsureUserAsync(
                userManager,
                "admin@matchboard.local",
                "Admin@123",
                "Module Leader",
                "Admin");

            if (!await context.Projects.AnyAsync())
            {
                context.Projects.AddRange(
                    new Project
                    {
                        Title = "Smart traffic prediction using machine learning",
                        Abstract = "A research project that predicts congestion patterns using historical route data, weather conditions, and time-based traffic signals to improve urban traffic flow.",
                        TechnicalStack = "ASP.NET Core, Python, SQL Server, scikit-learn",
                        StudentId = studentOne.Id,
                        ResearchAreaId = 1,
                        Status = ProjectStatus.Pending,
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    new Project
                    {
                        Title = "Secure final-year project review portal",
                        Abstract = "A web-based portal for managing proposal reviews, approval workflows, audit trails, and access control with a focus on secure supervisor interactions.",
                        TechnicalStack = "ASP.NET Core MVC, SQL Server, Bootstrap",
                        StudentId = studentTwo.Id,
                        ResearchAreaId = 3,
                        Status = ProjectStatus.Pending,
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    new Project
                    {
                        Title = "Cloud-native internship matching platform",
                        Abstract = "A distributed platform that supports role-based matching between students and companies while tracking applications, evaluations, and placement outcomes.",
                        TechnicalStack = "ASP.NET Core, Azure, SQL Server, REST API",
                        StudentId = studentOne.Id,
                        ResearchAreaId = 5,
                        Status = ProjectStatus.Pending,
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    }
                );

                await context.SaveChangesAsync();
            }
        }

        private static async Task<ApplicationUser> EnsureUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string fullName,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Could not seed user {email}: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }

            return user;
        }
    }
}
