using System;
using System.Collections.Generic;

namespace StudentManagementAPI.DTOs.Admin
{
    public class AdminStudentDetailDTO
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }


        public int TotalCourses { get; set; }


        public List<AdminStudentEnrollmentDTO> Enrollments { get; set; }

    }


    public class AdminStudentEnrollmentDTO
    {

        public int EnrollmentId { get; set; }


        public string CourseName { get; set; }


        public string ClassName { get; set; }


        public string TeacherName { get; set; }



        public string EnrollmentStatus { get; set; }


        public string PaymentStatus { get; set; }



        public decimal Price { get; set; }


        public DateTime EnrolledAt { get; set; }

    }
}