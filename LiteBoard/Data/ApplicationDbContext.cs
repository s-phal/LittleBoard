using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LiteBoard.Models;

namespace LiteBoard.Data
{
    public class ApplicationDbContext : IdentityDbContext<Member>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<LiteBoard.Models.Project> Project { get; set; } = default!;
        public DbSet<LiteBoard.Models.Chore> Chore { get; set; } = default!;
        public DbSet<LiteBoard.Models.Activity> Activity { get; set; } = default!;
    }
}