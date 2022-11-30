using System.ComponentModel.DataAnnotations;

namespace LiteBoard.Models
{
    public class ActivityModel
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;


        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }

    }
}
