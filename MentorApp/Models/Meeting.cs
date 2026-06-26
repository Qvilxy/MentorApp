using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MentorApp.Models
{
    [Table("Meetings")]
    public class Meeting
    {
        public int MeetingID { get; set; }
        public int ApplicationID { get; set; }
        public DateTime MeetingDate { get; set; }
        public string Topic { get; set; }
        public string Status { get; set; }

        public virtual Application Application { get; set; }
    }
}