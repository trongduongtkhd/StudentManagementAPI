using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Services.Interfaces;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
namespace StudentManagementAPI.Services.Iplementations
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDTO> GetDashboardAsync()
        {
            var totalStudents = await _context.Users
                .Where(u => u.Role == "User")
                .CountAsync();

            var totalCourses = await _context.Courses.CountAsync();

            var totalEnrollments = await _context.StudentCourses.CountAsync();

            var courseStats = await _context.Courses
                .Select(c => new CourseStatDTO
                {
                    CourseName = c.Name,
                    StudentCount = c.StudentCourses.Count()
                })
                .ToListAsync();

            return new DashboardDTO
            {
                TotalStudents = totalStudents,
                TotalCourses = totalCourses,
                TotalEnrollments = totalEnrollments,
                CourseStats = courseStats
            };
        }
    }
  }

