using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MentorApp.Models
{
    [Table("Mentors")]
    public class Mentor
    {
        [Key]
        public int MentorID { get; set; }
        public int UserID { get; set; }
        public int DirectionID { get; set; }
        public int? Experience { get; set; }  // Может быть nullable
        public int ExperienceMonths { get; set; }  // НЕ nullable, но должен иметь значение
        public string Description { get; set; }
        public decimal? HourlyRate { get; set; }
        public bool IsAvailable { get; set; }

        public virtual User User { get; set; }
        public virtual Direction Direction { get; set; }
    }
}