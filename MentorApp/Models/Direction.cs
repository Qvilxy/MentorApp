using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MentorApp.Models
{
    [Table("Directions")]
    public class Direction
    {
        [Key]
        public int DirectionID { get; set; }
        public string DirectionName { get; set; }
    }
}