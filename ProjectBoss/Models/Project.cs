using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace ProjectBoss.Models
{
    
    public class Project
    {
        public int Id { get; set; }

        [StringLength(35, ErrorMessage ="Title must be at least 5 characters and can not exceed 35 characters.",MinimumLength = 5)]
        public string Title { get; set; }

		[StringLength(80, ErrorMessage = "Description must be at least 15 characters and can not exceed 80 characters.", MinimumLength = 15)]
		public string Description { get; set; }
        public string? Notes { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

		[DataType(DataType.DateTime)]
		public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        public string MemberId { get; set; }
        public virtual Member? Member { get; set; }

        public virtual ICollection<Chore> Chores { get; set; } = new HashSet<Chore>();
        public virtual ICollection<ActivityModel> Activities { get; set; } = new HashSet<ActivityModel>();
        public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new HashSet<ProjectMember>();
    }
}
