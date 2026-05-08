using System.Collections.Generic;

namespace StudentManagementAPI.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}
