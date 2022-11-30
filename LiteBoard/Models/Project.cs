using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

// TODO Set limit max character limit on Description

namespace LiteBoard.Models
{
    
    public class Project
    {
        public int Id { get; set; }  
        public string Title { get; set; }    
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
    }
}
