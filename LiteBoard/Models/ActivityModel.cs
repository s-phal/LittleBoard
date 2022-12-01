using System.ComponentModel.DataAnnotations;

namespace LiteBoard.Models
{
    public class ActivityModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;


        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }

        //public int ChoreId { get; set; }
        //public virtual Chore? Chore { get; set; }

		public string MemberId { get; set; }
		public virtual Member? Member { get; set; }

	}
}

// TODO comment out Member if we dont need it

