using System;

namespace StudentManagementAPI.DTOs.Enrollments
{
    public class MyEnrollmentDTO
    {
        public int EnrollmentId { get; set; }


        public int CourseClassId { get; set; }


        public string CourseName { get; set; }


        public string ClassName { get; set; }


        public string Status { get; set; }


        public DateTime EnrolledAt { get; set; }


        public decimal Price { get; set; }


        public string PaymentStatus { get; set; }
    }
}
