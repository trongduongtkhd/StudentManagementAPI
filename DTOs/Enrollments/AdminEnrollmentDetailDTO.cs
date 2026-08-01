using System;

namespace StudentManagementAPI.DTOs.Enrollments
{
    public class AdminEnrollmentDetailDTO
    {
        // Enrollment

        public int EnrollmentId { get; set; }

        public string EnrollmentStatus { get; set; }

        public DateTime EnrolledAt { get; set; }



        // Student

        public int StudentId { get; set; }

        public string StudentName { get; set; }

        public string Username { get; set; }

        public int Age { get; set; }



        // Course

        public int CourseId { get; set; }

        public string CourseName { get; set; }

        // sschedule 
        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
        // Class

        public int CourseClassId { get; set; }

        public string ClassName { get; set; }



        // Payment

        public string PaymentCode { get; set; }

        public decimal Amount { get; set; }

        public string PaymentStatus { get; set; }

        public DateTime? PaidAt { get; set; }

        public string TeacherName { get; set; }
    }
}
