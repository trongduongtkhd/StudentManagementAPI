using StudentManagementAPI.Enums;
using System;
using System.Collections.Generic;

namespace StudentManagementAPI.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        // STUDENT
        public int StudentId { get; set; }  
        public Student Student { get; set; }
        // COURSE CLASS
        public int CourseClassId { get; set; }
        public CourseClass CourseClass { get; set; }
        // STATUS
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrolledAt { get; set; } 
        public ICollection<PaymentItem> PaymentItems { get; set; } = new List<PaymentItem>();
    }
}
