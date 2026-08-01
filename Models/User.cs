using System.Collections.Generic;

namespace StudentManagementAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public Teacher Teacher { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // "Admin" hoặc "User"
                                         // 👉 THÊM
        public string Name { get; set; }
        public int Age { get; set; }

        // 👉 Navigation
        public ICollection<Enrollment> Enrollments { get; set; }

        // 1 User → nhiều Payments

        public ICollection<Payment> Payments { get; set; }
    = new List<Payment>();

        // 1 User → nhiều Notifications

        public ICollection<Notification> Notifications { get; set; }
    = new List<Notification>();
    }
}
