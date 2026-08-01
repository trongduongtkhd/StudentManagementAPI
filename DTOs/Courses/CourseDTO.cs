using System;
using System.Collections.Generic;

namespace StudentManagementAPI.DTOs.Courses
{
    public class CourseDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int TotalStudents { get; set; }

        public int TotalClasses { get; set; }
        public List<CourseClassSummaryDTO> Classes { get; set; }
    }
    public class CourseClassSummaryDTO
    {
        public int Id { get; set; }

        public string ClassName { get; set; }

        public string TeacherName { get; set; }


        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }


        public int CurrentStudents { get; set; }

        public int MaxStudents { get; set; }

        public bool IsFull { get; set; }
    }
}
