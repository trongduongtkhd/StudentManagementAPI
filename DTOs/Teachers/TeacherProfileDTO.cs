using System;

namespace StudentManagementAPI.DTOs.Teachers
{
    public class TeacherProfileDTO
    {
        // User

        public int UserId { get; set; }

        public string TeacherCode { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime JoinDate { get; set; }

        public bool IsActive { get; set; }

        // Teacher

        public string Specialization { get; set; }

        public int YearsOfExperience { get; set; }

        public string Bio { get; set; }
    }
}
