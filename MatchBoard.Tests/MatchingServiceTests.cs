using FluentAssertions;
using MatchBoard.Web.Data;
using MatchBoard.Web.Models.Domain;
using MatchBoard.Web.Models.ViewModels;
using MatchBoard.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace MatchBoard.Tests
{
    public class MatchingServiceTests
    {
        [Fact]
        public async Task GetBlindProjectsForSupervisorAsync_FiltersByExpertise_AndHidesStudentIdentity()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var student = new ApplicationUser
            {
                Id = "student-1",
                UserName = "student@test.local",
                Email = "student@test.local",
                FullName = "Hidden Student"
            };

            var supervisor = new ApplicationUser
            {
                Id = "supervisor-1",
                UserName = "supervisor@test.local",
                Email = "supervisor@test.local",
                FullName = "Supervisor User"
            };

            context.Users.AddRange(student, supervisor);
            context.ResearchAreas.AddRange(
                new ResearchArea { Id = 1, Name = "Artificial Intelligence", Tag = "AI", IsActive = true },
                new ResearchArea { Id = 2, Name = "Cybersecurity", Tag = "CYBER", IsActive = true });
            context.SupervisorExpertises.Add(new SupervisorExpertise { SupervisorId = supervisor.Id, ResearchAreaId = 1 });
            context.Projects.AddRange(
                new Project
                {
                    Id = 101,
                    Title = "AI project title",
                    Abstract = "This abstract is intentionally long enough to satisfy validation and still describe an AI project in meaningful detail.",
                    TechnicalStack = "ASP.NET Core, ML.NET",
                    ResearchAreaId = 1,
                    StudentId = student.Id,
                    Status = ProjectStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                },
                new Project
                {
                    Id = 102,
                    Title = "Cyber project title",
                    Abstract = "This abstract is intentionally long enough to satisfy validation and still describe a cybersecurity project in meaningful detail.",
                    TechnicalStack = "ASP.NET Core, Burp Suite",
                    ResearchAreaId = 2,
                    StudentId = student.Id,
                    Status = ProjectStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                });

            await context.SaveChangesAsync();

            var service = new MatchingService(context);
            var results = await service.GetBlindProjectsForSupervisorAsync(supervisor.Id);

            results.Should().HaveCount(1);
            results.Single().ProjectId.Should().Be(101);
            results.Single().ResearchArea.Should().Be("Artificial Intelligence");

            typeof(BlindReviewViewModel).GetProperty("StudentId").Should().BeNull();
            typeof(BlindReviewViewModel).GetProperty("StudentName").Should().BeNull();
            typeof(BlindReviewViewModel).GetProperty("StudentEmail").Should().BeNull();
        }
    }
}
