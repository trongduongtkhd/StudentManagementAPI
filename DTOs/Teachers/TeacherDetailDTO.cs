using System;
using System.Collections.Generic;

namespace StudentManagementAPI.DTOs.Teachers
{
    public class TeacherDetailDTO
    {
        public int Id { get; set; }
        public string TeacherCode { get; set; }
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        // NEW
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime JoinDate { get; set; }

        public bool IsActive { get; set; }
        // Professional
        public string Specialization { get; set; }

        public string Bio { get; set; }

        public int YearsOfExperience { get; set; }


        // Statistics
        public int TotalClasses { get; set; }

        public int TotalStudents { get; set; }


        public List<TeacherClassDTO> Classes { get; set; }
    }
}
