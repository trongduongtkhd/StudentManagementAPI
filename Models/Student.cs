using System;
using System.Collections.Generic;

namespace StudentManagementAPI.Models
{
    public class Student
    {
        public int Id { get; set; }
        // =========================
        // ACCOUNT
        // =========================
        public int UserId { get; set; }
        public User User { get; set; }
        // =========================
        // STUDENT PROFILE
        // =========================
        public string StudentCode { get; set; }
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
        // =========================
        // STUDENT BUSINESS
        // =========================
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
