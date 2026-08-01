using System;
using System.Collections.Generic;

namespace StudentManagementAPI.DTOs.Teachers
{
    public class TeacherDetailDTO
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
        // them 
        public int TotalClasses { get; set; }

        public int TotalStudents { get; set; }


        public List<TeacherClassDTO> Classes { get; set; }
    }
}
