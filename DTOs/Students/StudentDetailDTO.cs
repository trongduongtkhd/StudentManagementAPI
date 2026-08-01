using System;
using System.Collections.Generic;

namespace StudentManagementAPI.DTOs.Students
{
    public class StudentDetailDTO
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public string Role { get; set; }

        public int TotalEnrollments { get; set; }

        public int ActiveEnrollments { get; set; }

        public List<StudentEnrollmentDTO> Enrollments { get; set; }
    }
    public class StudentEnrollmentDTO
    {
        public int EnrollmentId { get; set; }

        public string CourseName { get; set; }

        public string ClassName { get; set; }

        public string Status { get; set; }

        public DateTime EnrolledAt { get; set; }

        public string PaymentStatus { get; set; }

        public decimal Amount { get; set; }

        public DateTime? PaidAt { get; set; }
    }
}
