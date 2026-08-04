using System;

namespace StudentManagementAPI.DTOs.Teachers
{
    public class UpdateTeacherProfileDTO
    {
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Bio { get; set; }
    }
}
