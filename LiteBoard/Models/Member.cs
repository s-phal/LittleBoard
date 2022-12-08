using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBoss.Models
{
    public class Member : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }


        public string? AvatarUrl { get; set; } = "default.png";
        [NotMapped]
        public IFormFile? ImageFile { get; set; }


        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();

    }
}
