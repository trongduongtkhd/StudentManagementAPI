using System;
using System.Collections.Generic;

namespace StudentManagementAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string StudentCode { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }                               
        public string Name { get; set; }


        public int Age { get; set; }
        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Address { get; set; }

        public string? AvatarUrl { get; set; }

        public DateTime JoinDate { get; set; }

        public bool IsActive { get; set; }
        public Teacher Teacher { get; set; }

        // 👉 Navigation
        public ICollection<Enrollment> Enrollments { get; set; }

        // 1 User → nhiều Payments

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        // 1 User → nhiều Notifications
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    }
}
