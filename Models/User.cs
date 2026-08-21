using System;
using System.Collections.Generic;

namespace StudentManagementAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }                               

        public Student Student { get; set; }
        public Teacher Teacher { get; set; }

        // 1 User → nhiều Payments
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        // 1 User → nhiều Notifications
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    }
}
