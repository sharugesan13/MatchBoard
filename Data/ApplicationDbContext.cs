using MatchBoard.Web.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MatchBoard.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ResearchArea> ResearchAreas => Set<ResearchArea>();
        public DbSet<SupervisorExpertise> SupervisorExpertises => Set<SupervisorExpertise>();
        public DbSet<Match> Matches => Set<Match>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Student)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.ResearchArea)
                .WithMany(r => r.Projects)
                .HasForeignKey(p => p.ResearchAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Project)
                .WithMany()
                .HasForeignKey(m => m.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Supervisor)
                .WithMany(u => u.SupervisedMatches)
                .HasForeignKey(m => m.SupervisorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SupervisorExpertise>()
                .HasOne(se => se.Supervisor)
                .WithMany(u => u.Expertises)
                .HasForeignKey(se => se.SupervisorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SupervisorExpertise>()
                .HasOne(se => se.ResearchArea)
                .WithMany(r => r.SupervisorExpertises)
                .HasForeignKey(se => se.ResearchAreaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResearchArea>().HasData(
                new ResearchArea { Id = 1, Name = "Artificial Intelligence", Tag = "AI", IsActive = true },
                new ResearchArea { Id = 2, Name = "Web Development", Tag = "WEB", IsActive = true },
                new ResearchArea { Id = 3, Name = "Cybersecurity", Tag = "CYBER", IsActive = true },
                new ResearchArea { Id = 4, Name = "Machine Learning", Tag = "ML", IsActive = true },
                new ResearchArea { Id = 5, Name = "Cloud Computing", Tag = "CLOUD", IsActive = true },
                new ResearchArea { Id = 6, Name = "Mobile Development", Tag = "MOBILE", IsActive = true }
            );
        }
    }
}
