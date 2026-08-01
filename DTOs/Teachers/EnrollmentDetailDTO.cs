using System;

namespace StudentManagementAPI.DTOs.Teachers
{
    public class EnrollmentDetailDTO
    {
        public int StudentId { get; set; }


        public string Username { get; set; }


        public string StudentName { get; set; }



        // Enrollment

        public string EnrollmentStatus { get; set; }


        public DateTime EnrolledAt { get; set; }

        public string CourseName { get; set; } = string.Empty;

        // Payment

        public decimal Amount { get; set; }


        public string PaymentStatus { get; set; }


        public DateTime? PaidAt { get; set; }
    }
}
