using System;
using System.Collections.Generic;

namespace StudentManagementAPI.DTOs.Courses
{
    public class AdminClassDetailDTO
    {

        public int ClassId { get; set; }

        public string ClassName { get; set; }


        public string CourseName { get; set; }


        public string TeacherName { get; set; }


        public DayOfWeek DayOfWeek { get; set; }


        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }


        public int MaxStudents { get; set; }

        public int CurrentStudents { get; set; }


        public List<ClassStudentDTO> Students { get; set; }

    }


    public class ClassStudentDTO
    {

        public int StudentId { get; set; }

        public string StudentName { get; set; }

        public string Username { get; set; }


        public string EnrollmentStatus { get; set; }


        public string PaymentStatus { get; set; }


        public decimal Amount { get; set; }

    }
}
