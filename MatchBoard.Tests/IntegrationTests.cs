using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using MatchBoard.Web.Data;

namespace MatchBoard.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                var descriptors = services
                    .Where(d =>
                        d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                        d.ServiceType == typeof(AppDbContext))
                    .ToList();
                foreach (var d in descriptors)
                    services.Remove(d);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            });
        }
    }

    public class IntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public IntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task HomePage_Returns_200()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task LoginPage_Returns_200()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Account/Login");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task RegisterPage_Returns_200()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Account/Register");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ProjectIndex_Unauthenticated_Redirects()
        {
            var client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
            var response = await client.GetAsync("/Project/Index");
            ((int)response.StatusCode).Should().BeInRange(301, 302);
        }

        [Fact]
        public async Task AdminDashboard_Unauthenticated_Redirects()
        {
            var client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
            var response = await client.GetAsync("/Admin/Dashboard");
            ((int)response.StatusCode).Should().BeInRange(301, 302);
        }
    }
}