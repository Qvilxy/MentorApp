using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MentorApp.Models
{
    [Table("Reviews")]
    public class Review
    {
        public int ReviewID { get; set; }
        public int MentorID { get; set; }
        public int MenteeID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }

        public virtual Mentor Mentor { get; set; }
        public virtual Mentee Mentee { get; set; }
    }
}