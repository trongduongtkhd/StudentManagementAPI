using System;

namespace StudentManagementAPI.Models
{
    public class StudentCourse
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
