using StudentManagementAPI.Enums;
using System;

namespace StudentManagementAPI.DTOs.Courses
{
    public class CourseClassDTO
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public decimal Price { get; set; }
        public string CourseName { get; set; }

        public string ClassName { get; set; }
        public DateTime EnrolledAt { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string Session { get; set; }
        public int MaxStudents { get; set; }

        public int CurrentStudents { get; set; }
  
        public int RemainingSlots { get; set; }
        public bool IsFull { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public int? TeacherId { get; set; }  
        public string TeacherName { get; set; }
    }
}
