
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs.CourseClasses;
using StudentManagementAPI.DTOs.Courses;
using StudentManagementAPI.DTOs.Courses;
using StudentManagementAPI.DTOs.Teachers;
using StudentManagementAPI.Enums;
using StudentManagementAPI.Exceptions;
using StudentManagementAPI.Exceptions;
using StudentManagementAPI.Models;
using StudentManagementAPI.Services.Interfaces;
using StudentManagementAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks;
namespace StudentManagementAPI.Services.Iplementations
{
    public class CourseClassService:ICourseClasses
    {
        private readonly AppDbContext _context;

        public CourseClassService(AppDbContext context)
        {
            _context = context;

        }
        public async Task AssignTeacherAsync(int classId, int teacherId)
        {
            var courseClass = await _context.CourseClasses
                .FirstOrDefaultAsync(x => x.Id == classId);

            if (courseClass == null)
                throw new NotFoundException("Class không tồn tại");

            var teacher = await _context.Teachers
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == teacherId);

            if (teacher == null)
                throw new NotFoundException("Teacher không tồn tại");

            if (!teacher.IsActive)
                throw new BadRequestException("Teacher đã bị khóa");

            courseClass.TeacherId = teacher.Id;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CourseClassDTO>> GetClassesByCourseIdAsync(int courseId)
        {
            // =========================
            // CHECK COURSE EXISTS
            // =========================
            var courseExists = await _context.Courses
                .AnyAsync(c => c.Id == courseId);

            if (!courseExists)
                throw new NotFoundException(
                    "Course không tồn tại");

            // =========================
            // GET CLASSES
            // =========================
            var classes = await _context.CourseClasses
                .Where(cc => cc.CourseId == courseId)

                // =========================
                // FIX:
                // Include StudentCourses
                // để tính availability
                // =========================
                .Include(cc => cc.Enrollments)

                .Include(cc => cc.Course)
                 .Include(cc => cc.Teacher)
                  .ThenInclude(t => t.User)
                .ToListAsync();

            // =========================
            // MAP DTO
            // =========================
            return classes.Select(cc =>
            {
                // =========================
                // FIX:
                // Không count Cancelled
                // =========================
                var currentStudents = cc.Enrollments.Count(sc => sc.Status != EnrollmentStatus.Cancelled);

                return new CourseClassDTO
                {
                    Id = cc.Id,

                    CourseId = cc.CourseId,

                    CourseName = cc.Course.Name,
                    TeacherId = cc.TeacherId,

                    TeacherName = cc.Teacher != null ? cc.Teacher.Name : "Chưa phân công",

                    ClassName = cc.ClassName,
                    Price = cc.Course.Price,

                    DayOfWeek = cc.DayOfWeek,

                    StartDate = cc.StartDate,

                    EndDate = cc.EndDate,

                    StartTime = cc.StartTime,

                    EndTime = cc.EndTime,

                    Session = cc.Session,

                    MaxStudents = cc.MaxStudents,

                    CurrentStudents = currentStudents,

                    // =========================
                    // FIX:
                    // Remaining slots
                    // =========================
                    RemainingSlots = cc.MaxStudents - currentStudents,
                    // =========================
                    // FIX:
                    // Full logic
                    // =========================
                    IsFull = currentStudents >= cc.MaxStudents
                };
            });
        }


        public async Task<IEnumerable<CourseClassStudentDTO>> GetStudentsInClassAsync(int classId)
        {
            var classExists = await _context.CourseClasses
                .AnyAsync(x => x.Id == classId);

            if (!classExists)
                throw new NotFoundException("Class không tồn tại");

            var enrollments = await _context.Enrollments

                .Where(e => e.CourseClassId == classId)

                .Include(e => e.Student)

                .ToListAsync();

            return enrollments.Select(e => new CourseClassStudentDTO
            {
                StudentId = e.Student.Id,

                StudentCode = e.Student.StudentCode,

                Name = e.Student.Name,

                EnrollmentStatus = e.Status.ToString()
            });
        }

    }
}
