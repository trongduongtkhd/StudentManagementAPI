using System;

namespace StudentManagementAPI.DTOs.Teachers
{
    public class TeacherProfileDTO
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public string Specialization { get; set; }

        public string Bio { get; set; }

        public int YearsOfExperience { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
