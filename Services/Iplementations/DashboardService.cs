using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.DTOs.Dashboard;
using StudentManagementAPI.Enums;
using StudentManagementAPI.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagementAPI.Services.Iplementations
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardDTO>
      GetAdminDashboardAsync()
        {
            return new AdminDashboardDTO
            {
                TotalStudents =
                    await _context.Users
                        .CountAsync(u => u.Role == "User"),

                TotalCourses =
                    await _context.Courses.CountAsync(),

                TotalClasses =
                    await _context.CourseClasses.CountAsync(),

                PendingEnrollments =
                    await _context.StudentCourses
                        .CountAsync(sc =>
                            sc.Status == EnrollmentStatus.Pending),

                PaidEnrollments =
                    await _context.StudentCourses
                        .CountAsync(sc =>
                            sc.Status == EnrollmentStatus.Paid),

                CompletedEnrollments =
                    await _context.StudentCourses
                        .CountAsync(sc =>
                            sc.Status == EnrollmentStatus.Completed),

                TotalRevenue =
                    await _context.Payments
                        .Where(p => p.Status == PaymentStatus.Paid)
                        .SumAsync(p => (decimal?)p.TotalAmount) ?? 0,

                PendingPayments =
                    await _context.Payments
                        .CountAsync(p =>
                            p.Status == PaymentStatus.Pending)
            };
        }

        public async Task<UserDashboardDTO>
            GetUserDashboardAsync(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == username);

            if (user == null)
                throw new Exception("User không tồn tại");

            return new UserDashboardDTO
            {
                MyCourses =
       await _context.StudentCourses
           .CountAsync(sc =>
               sc.UserId == user.Id),

                PendingPayments =
       await _context.Payments
           .CountAsync(p =>
               p.UserId == user.Id &&
               p.Status == PaymentStatus.Pending)
            };
        }
    }
}