using System;

namespace StudentManagementAPI.DTOs.Enrollments
{
    public class AdminEnrollmentDTO
    {
        public int Id { get; set; }


        // Student

        public int StudentId { get; set; }

        public string StudentName { get; set; }

        public string Username { get; set; }


        // Course

        public int CourseId { get; set; }

        public string CourseName { get; set; }


        // Class

        public int CourseClassId { get; set; }

        public string ClassName { get; set; }



        // Status

        public string EnrollmentStatus { get; set; }


        // Payment

        public string PaymentStatus { get; set; }


        public decimal Amount { get; set; }


        public DateTime EnrolledAt { get; set; }
    }
}
