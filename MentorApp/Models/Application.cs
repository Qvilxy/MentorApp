using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MentorApp.Models
{
    [Table("Applications")]
    public class Application
    {
        public int ApplicationID { get; set; }
        public int MenteeID { get; set; }
        public int MentorID { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Comment { get; set; }

        public virtual Mentee Mentee { get; set; }
        public virtual Mentor Mentor { get; set; }
    }
}