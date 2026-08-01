using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.DTOs.Admin;
using StudentManagementAPI.DTOs.Courses;
using StudentManagementAPI.DTOs.Schedule;
using StudentManagementAPI.DTOs.Students;
using StudentManagementAPI.DTOs.Teachers;
using StudentManagementAPI.DTOs.Users;
using StudentManagementAPI.Enums;
using StudentManagementAPI.Exceptions;
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
            .Include(u => u.Enrollments)
                .ThenInclude(sc => sc.CourseClass)
                    .ThenInclude(cc => cc.Course)
            .Select(u => new UserDTO
            {
                Id = u.Id,
                Name = u.Name,
                Age = u.Age,
                Courses = u.Enrollments
                    .Select(sc => sc.CourseClass.Course.Name)
                    .ToList()
            })
            .ToListAsync();
        }

        // 👉 ADMIN: lấy theo ID
        public async Task<StudentDetailDTO> GetByIdAsync(int id)
        {
            var user = await _context.Users

         .Include(u => u.Enrollments)

             .ThenInclude(e => e.CourseClass)

                 .ThenInclude(c => c.Course)

         .Include(u => u.Enrollments)

             .ThenInclude(e => e.PaymentItems)

                 .ThenInclude(p => p.Payment)

         .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new NotFoundException("Student không tồn tại");
            return new StudentDetailDTO
            {
                Id = user.Id,

                Username = user.Username,

                Name = user.Name,

                Age = user.Age,

                Role = user.Role.ToString(),

                TotalEnrollments = user.Enrollments.Count,

                ActiveEnrollments = user.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled),

                Enrollments = user.Enrollments.Select(e =>
                {
                    var payment = e.PaymentItems.OrderByDescending(p => p.Payment.CreatedAt).FirstOrDefault();
                    return new StudentEnrollmentDTO
                    {
                        EnrollmentId = e.Id,

                        CourseName = e.CourseClass.Course.Name,

                        ClassName = e.CourseClass.ClassName,

                        Status = e.Status.ToString(),

                        EnrolledAt = e.EnrolledAt,

                        PaymentStatus =
                            payment?.Payment.Status.ToString() ?? "Pending",

                        Amount = payment?.Price ?? 0,

                        PaidAt = payment?.Payment.PaidAt
                    };
                }).ToList()
            };
        }

        public async Task<AdminStudentDetailDTO> GetStudentDetailAsync(int id)
        {

            var student = await _context.Users

                .Include(u => u.Enrollments)

                    .ThenInclude(e => e.CourseClass)

                        .ThenInclude(cc => cc.Course)

                .Include(u => u.Enrollments)

                    .ThenInclude(e => e.CourseClass)

                        .ThenInclude(cc => cc.Teacher)

                            .ThenInclude(t => t.User)

                .Include(u => u.Enrollments)

                    .ThenInclude(e => e.PaymentItems)

                        .ThenInclude(pi => pi.Payment)

                .FirstOrDefaultAsync(u =>
                    u.Id == id &&
                    u.Role == "User");

            if (student == null) throw new NotFoundException("Student không tồn tại");

            return new AdminStudentDetailDTO
            {

                Id = student.Id,

                Username = student.Username,

                Name = student.Name,

                Age = student.Age,

                TotalCourses = student.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled),

                Enrollments = student.Enrollments.Select(e => new AdminStudentEnrollmentDTO
                {

                    EnrollmentId = e.Id,

                    CourseName = e.CourseClass.Course.Name,

                    ClassName = e.CourseClass.ClassName,

                    TeacherName = e.CourseClass.Teacher != null ? e.CourseClass.Teacher.User.Name : "Chưa phân công",

                    EnrollmentStatus = e.Status.ToString(),

                    PaymentStatus = e.PaymentItems

                        .OrderByDescending(pi => pi.Payment.CreatedAt)

                        .Select(pi => pi.Payment.Status.ToString()).FirstOrDefault() ?? "Pending",
                     
                    Price = e.CourseClass.Price,

                    EnrolledAt = e.EnrolledAt

                }).ToList()
            };

        }

        public async Task<IEnumerable<AvailableTeacherDTO>> GetAvailableTeachersAsync()
        {
            return await _context.Users

                .Where(u => u.Role == "Teacher")

                .Where(u => !_context.Teachers.Any(t => t.UserId == u.Id))

                .Select(u => new AvailableTeacherDTO
                {
                    UserId = u.Id,

                    Username = u.Username,

                    Name = u.Name
                }).ToListAsync();
        }


        public async Task<TeacherAccountDTO> CreateTeacherAccountAsync(CreateTeacherAccountDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new BadRequestException("Username không được để trống");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new BadRequestException("Password không được để trống");

            if (dto.Password.Length < 6)
                throw new BadRequestException("Password phải từ 6 ký tự");

            var exists = await _context.Users
                .AnyAsync(x => x.Username == dto.Username);

            if (exists)
                throw new BadRequestException("Username đã tồn tại");

            var user = new User
            {
                Username = dto.Username,

                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),

                Name = dto.Name,

                Age = dto.Age,

                Role = "Teacher"
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();
            return new TeacherAccountDTO
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Age = user.Age,
                Role = user.Role
            };
        }

        public async Task<IEnumerable<ScheduleDTO>> GetMyScheduleAsync(string username)
        {
            var student = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (student == null)
                throw new NotFoundException("Student không tồn tại");

            var enrollments = await _context.Enrollments

              .Where(e =>
                 e.UserId == student.Id &&
                 e.Status != EnrollmentStatus.Cancelled)

              .Include(e => e.CourseClass)
               .ThenInclude(c => c.Course)

              .Include(e => e.CourseClass)
                .ThenInclude(c => c.Teacher)
             .ThenInclude(t => t.User)

             .Include(e => e.CourseClass)
              .ThenInclude(c => c.Enrollments)

            .ToListAsync();

            return enrollments.Select(e => new ScheduleDTO
            {
                CourseClassId = e.CourseClass.Id,

                CourseName = e.CourseClass.Course.Name,

                ClassName = e.CourseClass.ClassName,

                TeacherId = e.CourseClass.TeacherId ?? 0,

                TeacherName =
                    e.CourseClass.Teacher?.User?.Name ?? "Chưa phân công",

                DayOfWeek = e.CourseClass.DayOfWeek,

                StartTime = e.CourseClass.StartTime,

                EndTime = e.CourseClass.EndTime,

                StartDate = e.CourseClass.StartDate,

                EndDate = e.CourseClass.EndDate,

                CurrentStudents =
                    e.CourseClass.Enrollments.Count(x =>
                        x.Status != EnrollmentStatus.Cancelled),

                MaxStudents = e.CourseClass.MaxStudents
            });
        }

    }
}
