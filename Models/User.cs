using System.Collections.Generic;

namespace StudentManagementAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // "Admin" hoặc "User"
                                         // 👉 THÊM
        public string Name { get; set; }
        public int Age { get; set; }

        // 👉 Navigation
        public ICollection<StudentCourse> StudentCourses { get; set; }
    }
}
