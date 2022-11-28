using Microsoft.CodeAnalysis;

namespace LiteBoard.Models
{
	public class Chore
	{
		public int Id { get; set; }
		public string Body { get; set; }

		public int ProjectId { get; set; }
		public virtual Project? Project { get; set; }
	}
}
