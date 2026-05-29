using System;
using System.Collections.Generic;

namespace StudentManagementAPI.Models
{
    public class CourseClass
    {
        public int Id { get; set; }

        // FK
        public int CourseId { get; set; }

        public Course Course { get; set; }

        // Tên lớp
        public string ClassName { get; set; }

        // Monday, Tuesday...
        public DayOfWeek DayOfWeek { get; set; }

        // 01/06 -> 30/06
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        // 07:00
        public TimeSpan StartTime { get; set; }

        // 09:00
        public TimeSpan EndTime { get; set; }

        // Morning / Evening
        public string Session { get; set; }

        public int MaxStudents { get; set; }
        // gia 
        public decimal Price { get; set; }
        // Navigation
        public ICollection<StudentCourse> StudentCourses { get; set; }
            = new List<StudentCourse>();
    }
}
