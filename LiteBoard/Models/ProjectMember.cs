namespace ProjectBoss.Models
{
	public class ProjectMember
	{
		public int Id { get; set; }

		public int ProjectId { get; set; }
		public virtual Project? Project { get; set; }

		public string MemberId { get; set; }
		public virtual Member? Member { get; set; }

	}
}
