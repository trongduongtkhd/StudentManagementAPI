using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Enums;
using StudentManagementAPI.Models;
using StudentManagementAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagementAPI.Services.Iplementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // 👉 ADMIN: lấy tất cả student
        public async Task<IEnumerable<UserDTO>> GetAllStudentsAsync()
        {
            return await _context.Users
            .Where(u => u.Role == "User")
            .Include(u => u.StudentCourses)
                .ThenInclude(sc => sc.CourseClass)
                    .ThenInclude(cc => cc.Course)
            .Select(u => new UserDTO
            {
                Id = u.Id,
                Name = u.Name,
                Age = u.Age,
                Courses = u.StudentCourses
                    .Select(sc => sc.CourseClass.Course.Name)
                    .ToList()
            })
            .ToListAsync();
        }

        // 👉 ADMIN: lấy theo ID
        public async Task<UserDTO> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.StudentCourses)
                    .ThenInclude(sc => sc.CourseClass)
                        .ThenInclude(cc => cc.Course)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;

            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Age = user.Age,

                Courses = user.StudentCourses
                    .Select(sc => sc.CourseClass.Course.Name)
                    .ToList()
            };
        }

        // 👉 USER: chọn course (JWT)
        public async Task AssignCourseAsync(string username, int courseClassId)
        {
            // ================= USER =================
            var user = await _context.Users
                .Include(u => u.StudentCourses)
                    .ThenInclude(sc => sc.CourseClass)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                throw new Exception("User không tồn tại");

            // ================= CLASS =================
            var selectedClass = await _context.CourseClasses
                .Include(cc => cc.Course)
                .FirstOrDefaultAsync(cc => cc.Id == courseClassId);

            if (selectedClass == null)
                throw new Exception("Lớp học không tồn tại");

            // ================= CHECK CLASS FULL =================
            var currentStudents = await _context.StudentCourses
                .CountAsync(sc =>
                    sc.CourseClassId == courseClassId &&
                    sc.Status != EnrollmentStatus.Cancelled);

            if (currentStudents >= selectedClass.MaxStudents)
                throw new Exception("Lớp học đã đầy");

            // ================= CHECK ALREADY ENROLLED =================
            var alreadyEnrolled = await _context.StudentCourses
                .AnyAsync(sc =>
                    sc.UserId == user.Id &&
                    sc.CourseClassId == courseClassId &&
                    sc.Status != EnrollmentStatus.Cancelled);

            if (alreadyEnrolled)
                throw new Exception("Bạn đã đăng ký lớp này");

            // ================= CHECK SAME COURSE =================
            var sameCourse = user.StudentCourses
                .Any(sc =>
                    sc.CourseClass.CourseId == selectedClass.CourseId &&
                    sc.Status != EnrollmentStatus.Cancelled);

            if (sameCourse)
                throw new Exception("Bạn đã đăng ký khóa học này rồi");

            // ================= CHECK SCHEDULE CONFLICT =================
            var hasConflict = user.StudentCourses.Any(sc =>

                // Ignore cancelled
                sc.Status != EnrollmentStatus.Cancelled

                // Same day
                && sc.CourseClass.DayOfWeek == selectedClass.DayOfWeek

                // Time overlap
                && sc.CourseClass.StartTime < selectedClass.EndTime
                && selectedClass.StartTime < sc.CourseClass.EndTime

                // Date overlap
                && sc.CourseClass.StartDate <= selectedClass.EndDate
                && selectedClass.StartDate <= sc.CourseClass.EndDate
            );

            if (hasConflict)
                throw new Exception("Lịch học bị trùng giờ");

            // ================= ENROLL =================
            var studentCourse = new StudentCourse
            {
                UserId = user.Id,

                CourseClassId = courseClassId,

                Status = EnrollmentStatus.Pending,

                CreatedAt = DateTime.UtcNow
            };

            _context.StudentCourses.Add(studentCourse);

            await _context.SaveChangesAsync();
        }



        // 👉 USER: bỏ course
        public async Task RemoveCourseAsync(string username, int courseClassId)
        {
            Console.WriteLine("===== REMOVE COURSE DEBUG =====");

            Console.WriteLine($"Username: {username}");

            Console.WriteLine($"CourseClassId from frontend: {courseClassId}");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                throw new Exception("User không tồn tại");

            Console.WriteLine($"UserId: {user.Id}");

            var allEnrollments = await _context.StudentCourses
                .Where(sc => sc.UserId == user.Id)
                .ToListAsync();

            Console.WriteLine($"Total Enrollments: {allEnrollments.Count}");

            foreach (var enrollment in allEnrollments)
            {
                Console.WriteLine(
                   
                    $"CourseClassId: {enrollment.CourseClassId} | " +
                    $"Status: {enrollment.Status}");
            }

            var studentCourse = await _context.StudentCourses
                .FirstOrDefaultAsync(sc =>
                    sc.UserId == user.Id &&
                    sc.CourseClassId == courseClassId);

            Console.WriteLine($"studentCourse == null : {studentCourse == null}");

            if (studentCourse == null)
                throw new Exception("Bạn chưa đăng ký lớp này");

            _context.StudentCourses.Remove(studentCourse);

            await _context.SaveChangesAsync();

            Console.WriteLine("REMOVE SUCCESS");
        }

        // 👉 USER: lấy course của chính mình
        public async Task<IEnumerable<CourseClassDTO>> GetMyCoursesAsync(string username)
        {
            var user = await _context.Users
                .Include(u => u.StudentCourses)
                    .ThenInclude(sc => sc.CourseClass)
                        .ThenInclude(cc => cc.Course)

                          .Include(u => u.StudentCourses)
        .ThenInclude(sc => sc.PaymentItems)
            .ThenInclude(pi => pi.Payment)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                throw new System.Exception("User không tồn tại");

            return user.StudentCourses
                .Select(sc => new CourseClassDTO
                {
                    Id = sc.CourseClass.Id,

                    CourseId = sc.CourseClass.CourseId,

                    CourseName = sc.CourseClass.Course.Name,

                    ClassName = sc.CourseClass.ClassName,
                    Price = sc.CourseClass.Course.Price,
                    Status = sc.Status.ToString(),
                    PaymentStatus =
    sc.PaymentItems
        .OrderByDescending(pi => pi.Payment.CreatedAt)
        .Select(pi => pi.Payment.Status.ToString())
        .FirstOrDefault() ?? "Pending",
                    DayOfWeek = sc.CourseClass.DayOfWeek,

                    StartDate = sc.CourseClass.StartDate,

                    EndDate = sc.CourseClass.EndDate,

                    StartTime = sc.CourseClass.StartTime,

                    EndTime = sc.CourseClass.EndTime,

                    Session = sc.CourseClass.Session
                })
                .ToList();
        }
    }
}
