using System;

namespace StudentManagementAPI.DTOs.Courses
{
    public class CreateCourseClassDTO
    {
        public int CourseId { get; set; }
        public int? TeacherId { get; set; }
        public string ClassName { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public DateTime StartDate { get; set; }
        public int MaxStudents { get; set; }
        public DateTime EndDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string Session { get; set; }
    }
}
