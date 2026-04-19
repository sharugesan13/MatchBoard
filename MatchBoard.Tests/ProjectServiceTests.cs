using Xunit;
using FluentAssertions;
using MatchBoard.Web.Models;

namespace MatchBoard.Tests
{
    public class ProjectServiceTests
    {
        [Fact]
        public void NewProject_ShouldHaveStatus_Pending()
        {
            var project = new Project
            {
                Title = "AI Study Planner",
                ResearchArea = "Machine Learning",
                TechStack = "Python, React"
            };

            project.Status.Should().Be("Pending");
        }

        [Fact]
        public void Project_Title_ShouldNotBeEmpty()
        {
            var project = new Project { Title = "Test Project" };
            project.Title.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void NewProject_IdentityRevealed_ShouldBeFalse()
        {
            var project = new Project();
            project.IdentityRevealed.Should().BeFalse();
        }
    }
}