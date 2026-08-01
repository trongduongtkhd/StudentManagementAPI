using System;
using System.Collections.Generic;

namespace StudentManagementAPI.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        public int UserId { get; set; }
       
        public User User { get; set; }

        public string Specialization { get; set; }

        public string Bio { get; set; }

        public int YearsOfExperience { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<CourseClass> CourseClasses { get; set; }
            = new List<CourseClass>();
    }
}
