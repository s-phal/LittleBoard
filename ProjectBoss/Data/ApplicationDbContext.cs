using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectBoss.Models;

namespace ProjectBoss.Data
{
    public class ApplicationDbContext : IdentityDbContext<Member>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ProjectBoss.Models.Project> Project { get; set; } = default!;
        public DbSet<ProjectBoss.Models.Chore> Chore { get; set; } = default!;
        public DbSet<ProjectBoss.Models.ActivityModel> Activity { get; set; } = default!;
        public DbSet<ProjectBoss.Models.ProjectMember> ProjectMember { get; set; } = default!;
	}
}