using System.ComponentModel.DataAnnotations.Schema;

namespace MentorApp.Models
{
    [Table("Mentees")]
    public class Mentee
    {
        public int MenteeID { get; set; }
        public int UserID { get; set; }
        public int DirectionID { get; set; }
        public string Goal { get; set; }

        public virtual User User { get; set; }
        public virtual Direction Direction { get; set; }
    }
}