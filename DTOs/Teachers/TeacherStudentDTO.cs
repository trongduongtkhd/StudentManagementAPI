using System;

namespace StudentManagementAPI.DTOs.Teachers
{
    public class TeacherStudentDTO
    {
        public int StudentId { get; set; }


        public string Username { get; set; }


        public string Name { get; set; }


        public string Email { get; set; }


        public string Status { get; set; }


        public DateTime EnrolledAt { get; set; }
    }
}
