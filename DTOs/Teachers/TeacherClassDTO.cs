using System;

namespace StudentManagementAPI.DTOs.Teachers
{
    public class TeacherClassDTO
    {
        public int ClassId { get; set; }


        public int CourseId { get; set; }


        public string CourseName { get; set; }


        public string ClassName { get; set; }


        public DayOfWeek DayOfWeek { get; set; }


        public DateTime StartDate { get; set; }


        public DateTime EndDate { get; set; }


        public TimeSpan StartTime { get; set; }


        public TimeSpan EndTime { get; set; }


        public int CurrentStudents { get; set; }


        public int MaxStudents { get; set; }


        public bool IsFull { get; set; }
    }
}
