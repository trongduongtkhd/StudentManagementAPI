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
    public class CourseService : ICourseService
    {
        private readonly AppDbContext _context;

        public CourseService(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET ALL COURSES
        // =========================================================
        public async Task<IEnumerable<CourseDTO>> GetAllAsync()
        {
            return await _context.Courses
                .Select(c => new CourseDTO
                {
                    Id = c.Id,

                    Name = c.Name,

                    Description = c.Description,

                    ImageUrl = c.ImageUrl,

                    // =========================
                    // FIX:
                    // Thêm TotalClasses
                    // =========================
                    TotalClasses = c.Classes.Count,

                    // =========================
                    // FIX:
                    // Không count Cancelled
                    // =========================
                    TotalStudents = c.Classes
                        .SelectMany(cl => cl.StudentCourses)
                        .Count(sc =>
                            sc.Status != EnrollmentStatus.Cancelled)
                })
                .ToListAsync();
        }

        // =========================================================
        // GET COURSE BY ID
        // =========================================================
        public async Task<CourseDTO> GetByIdAsync(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Classes)
                    .ThenInclude(cl => cl.StudentCourses)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return null;

            return new CourseDTO
            {
                Id = course.Id,

                Name = course.Name,

                Description = course.Description,

                ImageUrl = course.ImageUrl,

                // =========================
                // FIX:
                // Thêm TotalClasses
                // =========================
                TotalClasses = course.Classes.Count,

                // =========================
                // FIX:
                // Không count Cancelled
                // =========================
                TotalStudents = course.Classes
                    .SelectMany(cl => cl.StudentCourses)
                    .Count(sc =>
                        sc.Status != EnrollmentStatus.Cancelled)
            };
        }

        // =========================================================
        // CREATE COURSE
        // =========================================================
        public async Task<CourseDTO> CreateAsync(CreateCourseDTO dto)
        {
            // =========================
            // VALIDATE NAME
            // =========================
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception(
                    "Tên khóa học không được để trống");

            // =========================
            // CHECK DUPLICATE
            // =========================
            var exists = await _context.Courses
                .AnyAsync(c =>
                    c.Name.ToLower() ==
                    dto.Name.ToLower());

            if (exists)
                throw new Exception(
                    "Course đã tồn tại");

            // =========================
            // CREATE ENTITY
            // =========================
            var course = new Course
            {
                Name = dto.Name.Trim(),

                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl
            };

            // =========================
            // SAVE DATABASE
            // =========================
            _context.Courses.Add(course);

            await _context.SaveChangesAsync();

            // =========================
            // RETURN DTO
            // =========================
            return new CourseDTO
            {
                Id = course.Id,

                Name = course.Name,

                Description = course.Description,

                ImageUrl = course.ImageUrl,

                TotalClasses = 0,

                TotalStudents = 0
            };
        }

        // =========================================================
        // UPDATE COURSE
        // =========================================================
        public async Task<CourseDTO> UpdateAsync(
            int id,
            UpdateCourseDTO dto)
        {
            // =========================
            // FIND COURSE
            // =========================
            var course = await _context.Courses
                .Include(c => c.Classes)
                    .ThenInclude(cl => cl.StudentCourses)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                throw new Exception(
                    "Course không tồn tại");

            // =========================
            // VALIDATE NAME
            // =========================
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception(
                    "Tên khóa học không được để trống");

            // =========================
            // CHECK DUPLICATE
            // =========================
            var exists = await _context.Courses
                .AnyAsync(c =>
                    c.Name.ToLower() ==
                    dto.Name.ToLower()
                    && c.Id != id);

            if (exists)
                throw new Exception(
                    "Course đã tồn tại");

            // =========================
            // UPDATE ENTITY
            // =========================
            course.Name = dto.Name.Trim();

            course.Description = dto.Description;

            course.ImageUrl = dto.ImageUrl;
            course.Price = dto.Price;

            // =========================
            // SAVE DATABASE
            // =========================
            await _context.SaveChangesAsync();

            // =========================
            // RETURN DTO
            // =========================
            return new CourseDTO
            {
                Id = course.Id,

                Name = course.Name,

                Description = course.Description,

                ImageUrl = course.ImageUrl,

                TotalClasses = course.Classes.Count,
                Price = course.Price,

                // =========================
                // FIX:
                // Không count Cancelled
                // =========================
                TotalStudents = course.Classes
                    .SelectMany(cl => cl.StudentCourses)
                    .Count(sc =>
                        sc.Status != EnrollmentStatus.Cancelled)
            }; 
        }

        // =========================================================
        // DELETE COURSE
        // =========================================================
        public async Task<bool> DeleteAsync(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Classes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                throw new Exception(
                    "Course không tồn tại");

            // =========================
            // BUSINESS RULE:
            // Không cho xóa nếu đã có class
            // =========================
            if (course.Classes.Any())
                throw new Exception(
                    "Không thể xóa khóa học vì đã có lớp học");

            _context.Courses.Remove(course);

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // CREATE COURSE CLASS
        // =========================================================
        public async Task<CourseClassDTO>
            CreateCourseClassAsync(
                CreateCourseClassDTO dto)
        {
            // =========================
            // CHECK COURSE
            // =========================
            var course = await _context.Courses
                .FirstOrDefaultAsync(c =>
                    c.Id == dto.CourseId);

            if (course == null)
                throw new Exception(
                    "Course không tồn tại");

            // =========================
            // VALIDATE TIME
            // =========================
            if (dto.StartTime >= dto.EndTime)
                throw new Exception(
                    "StartTime phải nhỏ hơn EndTime");

            // =========================
            // VALIDATE DATE
            // =========================
            if (dto.StartDate > dto.EndDate)
                throw new Exception(
                    "StartDate không hợp lệ");

            // =========================
            // VALIDATE MAX STUDENTS
            // =========================
            if (dto.MaxStudents <= 0)
                throw new Exception(
                    "MaxStudents phải lớn hơn 0");

            // =========================
            // CHECK DUPLICATE CLASS
            // =========================
            var classExists = await _context.CourseClasses
                .AnyAsync(cc =>
                    cc.CourseId == dto.CourseId &&
                    cc.ClassName.ToLower() ==
                    dto.ClassName.ToLower());

            if (classExists)
                throw new Exception(
                    "Class đã tồn tại");

            // =========================
            // CREATE ENTITY
            // =========================
            var courseClass = new CourseClass
            {
                CourseId = dto.CourseId,

                ClassName = dto.ClassName,
                Price = course.Price,

                DayOfWeek = dto.DayOfWeek,

                StartDate = dto.StartDate,

                EndDate = dto.EndDate,

                StartTime = dto.StartTime,

                EndTime = dto.EndTime,

                Session = dto.Session,

                MaxStudents = dto.MaxStudents
            };

            // =========================
            // SAVE DATABASE
            // =========================
            _context.CourseClasses.Add(courseClass);

            await _context.SaveChangesAsync();

            // =========================
            // RETURN DTO
            // =========================
            return new CourseClassDTO
            {
                Id = courseClass.Id,

                CourseId = course.Id,

                CourseName = course.Name,

                ClassName = courseClass.ClassName,

                DayOfWeek = courseClass.DayOfWeek,

                StartDate = courseClass.StartDate,

                EndDate = courseClass.EndDate,

                StartTime = courseClass.StartTime,

                EndTime = courseClass.EndTime,

                Session = courseClass.Session,

                MaxStudents = courseClass.MaxStudents,

                CurrentStudents = 0,

                RemainingSlots =
                    courseClass.MaxStudents,

                IsFull = false
            };
        }

        // =========================================================
        // GET CLASSES BY COURSE ID
        // =========================================================
        public async Task<IEnumerable<CourseClassDTO>>
            GetClassesByCourseIdAsync(int courseId)
        {
            // =========================
            // CHECK COURSE EXISTS
            // =========================
            var courseExists = await _context.Courses
                .AnyAsync(c => c.Id == courseId);

            if (!courseExists)
                throw new Exception(
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
                .Include(cc => cc.StudentCourses)

                .Include(cc => cc.Course)

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
                var currentStudents =
                    cc.StudentCourses.Count(sc =>
                        sc.Status !=
                        EnrollmentStatus.Cancelled);

                return new CourseClassDTO
                {
                    Id = cc.Id,

                    CourseId = cc.CourseId,

                    CourseName = cc.Course.Name,

                    ClassName = cc.ClassName,
                    Price = cc.Course.Price,

                    DayOfWeek = cc.DayOfWeek,

                    StartDate = cc.StartDate,

                    EndDate = cc.EndDate,

                    StartTime = cc.StartTime,

                    EndTime = cc.EndTime,

                    Session = cc.Session,

                    MaxStudents = cc.MaxStudents,

                    CurrentStudents =
                        currentStudents,

                    // =========================
                    // FIX:
                    // Remaining slots
                    // =========================
                    RemainingSlots =
                        cc.MaxStudents -
                        currentStudents,

                    // =========================
                    // FIX:
                    // Full logic
                    // =========================
                    IsFull =
                        currentStudents >=
                        cc.MaxStudents
                };
            });
        }
        public async Task<IEnumerable<CourseClassDTO>> GetAllCourseClassesAsync()

        {
            var classes = await _context.CourseClasses
                .Include(cc => cc.Course)
                .Include(cc => cc.StudentCourses)
                .ToListAsync();

            return classes.Select(cc =>
            {
                var currentStudents =
                    cc.StudentCourses.Count(sc =>
                        sc.Status != EnrollmentStatus.Cancelled);

                return new CourseClassDTO
                {
                    Id = cc.Id,

                    CourseId = cc.CourseId,

                    CourseName = cc.Course.Name,

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

                    RemainingSlots =
                        cc.MaxStudents - currentStudents,

                    IsFull =
                        currentStudents >= cc.MaxStudents
                };
            });
        }

        public async Task<CourseClassDTO>
    UpdateCourseClassAsync(
        int id,
        UpdateCourseClassDTO dto)
        {
            var courseClass = await _context.CourseClasses
                .Include(cc => cc.Course)
                .Include(cc => cc.StudentCourses)
                .FirstOrDefaultAsync(cc => cc.Id == id);

            if (courseClass == null)
                throw new Exception("Class không tồn tại");

            // ================= VALIDATE =================

            if (dto.StartTime >= dto.EndTime)
                throw new Exception(
                    "StartTime phải nhỏ hơn EndTime");

            if (dto.StartDate > dto.EndDate)
                throw new Exception(
                    "StartDate không hợp lệ");

            if (dto.MaxStudents <= 0)
                throw new Exception(
                    "MaxStudents phải lớn hơn 0");

            // ================= UPDATE =================

            courseClass.ClassName = dto.ClassName;

            courseClass.DayOfWeek = dto.DayOfWeek;

            courseClass.StartDate = dto.StartDate;

            courseClass.EndDate = dto.EndDate;

            courseClass.StartTime = dto.StartTime;

            courseClass.EndTime = dto.EndTime;

            courseClass.Session = dto.Session;

            courseClass.MaxStudents = dto.MaxStudents;

            await _context.SaveChangesAsync();

            // ================= RETURN =================

            var currentStudents =
                courseClass.StudentCourses.Count(sc =>
                    sc.Status != EnrollmentStatus.Cancelled);

            return new CourseClassDTO
            {
                Id = courseClass.Id,

                CourseId = courseClass.CourseId,

                CourseName = courseClass.Course.Name,

                ClassName = courseClass.ClassName,
                Price = courseClass.Course.Price,

                DayOfWeek = courseClass.DayOfWeek,

                StartDate = courseClass.StartDate,

                EndDate = courseClass.EndDate,

                StartTime = courseClass.StartTime,

                EndTime = courseClass.EndTime,

                Session = courseClass.Session,

                MaxStudents = courseClass.MaxStudents,

                CurrentStudents = currentStudents,

                RemainingSlots =
                    courseClass.MaxStudents - currentStudents,

                IsFull =
                    currentStudents >= courseClass.MaxStudents
            };
        }

        public async Task<bool>
    DeleteCourseClassAsync(int id)
        {
            var courseClass = await _context.CourseClasses
                .Include(cc => cc.StudentCourses)
                .FirstOrDefaultAsync(cc => cc.Id == id);

            if (courseClass == null)
                throw new Exception("Class không tồn tại");

            // ================= CHECK ENROLLMENTS =================

            if (courseClass.StudentCourses.Any())
                throw new Exception(
                    "Không thể xóa lớp đã có học viên");

            _context.CourseClasses.Remove(courseClass);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}