using System;

namespace StudentManagementAPI.Models
{
    public class Notification
    {
        public int Id { get; set; }

        // USER
        public int UserId { get; set; }

        public User User { get; set; }

        // CONTENT
        public string Title { get; set; }

        public string Message { get; set; }

        // READ STATUS
        public bool IsRead { get; set; }

        // CREATED
        public DateTime CreatedAt { get; set; }

    }
}
