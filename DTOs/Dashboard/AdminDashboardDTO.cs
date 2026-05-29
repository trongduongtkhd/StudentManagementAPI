namespace StudentManagementAPI.DTOs.Dashboard
{
    public class AdminDashboardDTO
    {
        public int TotalStudents { get; set; }

        public int TotalCourses { get; set; }

        public int TotalClasses { get; set; }

        public int PendingEnrollments { get; set; }

        public int PaidEnrollments { get; set; }

        public int CompletedEnrollments { get; set; }

        public decimal TotalRevenue { get; set; }

        public int PendingPayments { get; set; }
    }
}

