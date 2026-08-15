using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.DTOs.Admin;
using StudentManagementAPI.DTOs.Schedule;
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
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        // 👉 ADMIN: lấy tất cả student
        public async Task<IEnumerable<StudentDTO>> GetAllStudentsAsync()
        {
            return await _context.Students
             .Include(s => s.User)
              .Select(s => new StudentDTO
         {
           Id = s.Id,
           StudentCode = s.StudentCode,
           Name = s.Name,
           IsActive = s.IsActive
         }).ToListAsync();
        }

        // 👉 ADMIN: lấy theo ID
        public async Task<StudentProfileDTO> GetByIdAsync(int id)
        {
            var student = await _context.Students
               .Include(s => s.User)
                 .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                throw new NotFoundException("Student không tồn tại");

            return new StudentProfileDTO
            {
                Id = student.Id,

                StudentCode = student.StudentCode,

                Username = student.User.Username,

                Name = student.Name,

                Email = student.Email,

                Phone = student.Phone,

                Address = student.Address,

                DateOfBirth = student.DateOfBirth,

                Gender = student.Gender,

                JoinDate = student.JoinDate,

                IsActive = student.IsActive
            };
        }



        public async Task<IEnumerable<ScheduleDTO>> GetMyScheduleAsync(string username)
        {
            var student = await _context.Students
            .Include(s => s.User)
              .FirstOrDefaultAsync(s => s.User.Username == username);

            if (student == null)
                throw new NotFoundException("Student không tồn tại");

            var enrollments = await _context.Enrollments

              .Where(e =>
                 e.StudentId == student.Id &&
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

                TeacherName = e.CourseClass.Teacher?.Name ?? "Chưa phân công",

                DayOfWeek = e.CourseClass.DayOfWeek,

                StartTime = e.CourseClass.StartTime,

                EndTime = e.CourseClass.EndTime,

                StartDate = e.CourseClass.StartDate,

                EndDate = e.CourseClass.EndDate,

                CurrentStudents = e.CourseClass.Enrollments.Count(x => x.Status != EnrollmentStatus.Cancelled),

                MaxStudents = e.CourseClass.MaxStudents
            });
        }


        public async Task<AdminStudentDetailDTO> GetStudentDetailAsync(int id)
        {
            var student = await _context.Students
             .Include(s => s.User)

             .Include(s => s.Enrollments)
              .ThenInclude(e => e.CourseClass)
               .ThenInclude(cc => cc.Course)

             .Include(s => s.Enrollments)
               .ThenInclude(e => e.CourseClass)
                .ThenInclude(cc => cc.Teacher)
                   .ThenInclude(t => t.User)

            .Include(s => s.Enrollments)
             .ThenInclude(e => e.PaymentItems)
               .ThenInclude(pi => pi.Payment)

              .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) throw new NotFoundException("Student không tồn tại");

            return new AdminStudentDetailDTO
            {

                Id = student.Id,

                Username = student.User.Username,

                Name = student.Name,

                Age = student.Age,

                TotalCourses = student.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled),

                Enrollments = student.Enrollments.Select(e => new AdminStudentEnrollmentDTO
                {

                    EnrollmentId = e.Id,

                    CourseName = e.CourseClass.Course.Name,

                    ClassName = e.CourseClass.ClassName,

                    TeacherName = e.CourseClass.Teacher != null ? e.CourseClass.Teacher.Name : "Chưa phân công",

                    EnrollmentStatus = e.Status.ToString(),

                    PaymentStatus = e.PaymentItems

                        .OrderByDescending(pi => pi.Payment.CreatedAt)

                        .Select(pi => pi.Payment.Status.ToString()).FirstOrDefault() ?? "Pending",
                     
                    Price = e.CourseClass.Price,

                    EnrolledAt = e.EnrolledAt

                }).ToList()
            };

        }



        

        // Profile 
        public async Task<StudentProfileDTO> GetProfileAsync(string username)
        {
            var student = await _context.Students
             .Include(s => s.User)
               .FirstOrDefaultAsync(s => s.User.Username == username);
            if (student == null)
                throw new NotFoundException("Student không tồn tại");

            return new StudentProfileDTO
            {
                Id = student.Id,

                StudentCode = student.StudentCode,

                Username = student.User.Username,

                Name = student.Name,

                Email = student.Email,

                Phone = student.Phone,

                Address = student.Address,

                DateOfBirth = student.DateOfBirth,

                Gender = student.Gender,

                JoinDate = student.JoinDate,

                IsActive = student.IsActive
            };
        }

        public async Task UpdateProfileAsync(string username, UpdateStudentProfileDTO dto)
        {
            var student = await _context.Students
             .Include(s => s.User)
               .FirstOrDefaultAsync(s => s.User.Username == username);

            if (student == null)
                throw new NotFoundException("Student không tồn tại");
            student.Name = dto.Name;
            student.Email = dto.Email;
            student.Phone = dto.Phone;
            student.Address = dto.Address;
            student.DateOfBirth = dto.DateOfBirth;
            student.Gender = dto.Gender;

            await _context.SaveChangesAsync();
        }

    }
}
