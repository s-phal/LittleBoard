using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiteBoard.Models
{
	public class Chore
	{
		public int Id { get; set; }
		public string Body { get; set; }
		public bool Completed { get; set; } = false;
		[DataType(DataType.DateTime)]
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

		[DataType(DataType.DateTime)]
		public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

		public int ProjectId { get; set; }
		public virtual Project? Project { get; set; }
	}
}
