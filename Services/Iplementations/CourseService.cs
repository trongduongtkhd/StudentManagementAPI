using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs.Courses;
using StudentManagementAPI.DTOs.Teachers;
using StudentManagementAPI.Enums;
using StudentManagementAPI.Models;
using StudentManagementAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagementAPI.Exceptions;
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
                        .SelectMany(cl => cl.Enrollments)
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
                    .ThenInclude(cl => cl.Teacher)
                        .ThenInclude(t => t.User)

                .Include(c => c.Classes)
                    .ThenInclude(cl => cl.Enrollments)

                .FirstOrDefaultAsync(c => c.Id == id);


            if (course == null)
            {
                throw new NotFoundException(
                    "Course không tồn tại");
            }


            return new CourseDTO
            {
                Id = course.Id,

                Name = course.Name,

                Description = course.Description,

                ImageUrl = course.ImageUrl,

                Price = course.Price,
 
                TotalClasses = course.Classes.Count,

                TotalStudents = course.Classes.SelectMany(cl => cl.Enrollments).Count(e => e.Status != EnrollmentStatus.Cancelled),

                Classes = course.Classes.Select(cl =>
                {

                    var currentStudents = cl.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled);

                    return new CourseClassSummaryDTO
                        {
                            Id = cl.Id,

                            ClassName = cl.ClassName,

                            TeacherName = cl.Teacher != null ? cl.Teacher.Name : "Chưa phân công",

                            DayOfWeek = cl.DayOfWeek,

                            StartTime = cl.StartTime,

                            EndTime = cl.EndTime,

                            CurrentStudents = currentStudents,

                            MaxStudents = cl.MaxStudents,

                            IsFull = currentStudents >= cl.MaxStudents
                    };
                    }).ToList()
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
                throw new BadRequestException("Tên khóa học không được để trống");


            // =========================
            // CHECK DUPLICATE
            // =========================
            var exists = await _context.Courses
                .AnyAsync(c =>
                    c.Name.ToLower() ==
                    dto.Name.ToLower());

            if (exists)
                throw new BadRequestException(
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
                    .ThenInclude(cl => cl.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                throw new NotFoundException(
                    "Course không tồn tại");

            // =========================
            // VALIDATE NAME
            // =========================
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new BadRequestException(
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
                throw new BadRequestException("Course đã tồn tại");


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
                    .SelectMany(cl => cl.Enrollments)
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
                throw new NotFoundException(
                    "Course không tồn tại");

            // =========================
            // BUSINESS RULE:
            // Không cho xóa nếu đã có class
            // =========================
            if (course.Classes.Any())
                throw new BadRequestException(
                    "Không thể xóa khóa học vì đã có lớp học");

            _context.Courses.Remove(course);

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // CREATE COURSE CLASS
        // =========================================================
        public async Task<CourseClassDTO> CreateCourseClassAsync(CreateCourseClassDTO dto)
        {
            // =========================
            // CHECK COURSE
            // =========================
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == dto.CourseId);
            if (course == null)
                throw new NotFoundException("Course không tồn tại");
            // =========================
            // CHECK TEACHER
            // =========================
            Teacher teacher = null;
            if (dto.TeacherId.HasValue)
            {    
                teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == dto.TeacherId.Value);
                if (teacher == null)
                {
                    throw new NotFoundException(
                        "Teacher không tồn tại");
                }
                if (!teacher.IsActive)
                {
                    throw new BadRequestException("Teacher đang inactive");
                }
            }
            // =========================
            // VALIDATE TIME
            // =========================
            if (dto.StartTime >= dto.EndTime)
                throw new BadRequestException(
                    "StartTime phải nhỏ hơn EndTime");

            // =========================
            // VALIDATE DATE
            // =========================
            if (dto.StartDate > dto.EndDate)
                throw new BadRequestException(
                    "StartDate không hợp lệ");

            // =========================
            // VALIDATE MAX STUDENTS
            // =========================
            if (dto.MaxStudents <= 0)
                throw new BadRequestException(
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
                throw new BadRequestException(
                    "Class đã tồn tại");

            // =========================
            // CREATE ENTITY
            // =========================
            var courseClass = new CourseClass
            {
                CourseId = dto.CourseId,
                TeacherId = dto.TeacherId,
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
            var result = await _context.CourseClasses
                .Include(cc => cc.Course)
                  .Include(cc => cc.Teacher)
                    .ThenInclude(t => t.User)
                      .Include(cc => cc.Enrollments)
                         .FirstAsync(cc => cc.Id == courseClass.Id);
            var currentStudents = result.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled);

            // =========================
            // RETURN DTO
            // =========================
            return new CourseClassDTO
            {
                Id = result.Id,

                CourseId = result.CourseId,

                CourseName = result.Course.Name,

                TeacherId = result.TeacherId,

                TeacherName = result.Teacher?.Name ?? "Chưa phân công",

                ClassName = result.ClassName,

                Price = result.Price,

                DayOfWeek = result.DayOfWeek,

                StartDate = result.StartDate,

                EndDate = result.EndDate,

                StartTime = result.StartTime,

                EndTime = result.EndTime,

                Session = result.Session,

                MaxStudents = result.MaxStudents,

                CurrentStudents = currentStudents,

                RemainingSlots = result.MaxStudents - currentStudents,

                IsFull = currentStudents >= result.MaxStudents

            };
        }

        // =========================================================
        // GET CLASSES BY COURSE ID
        // =========================================================
      
        public async Task<IEnumerable<CourseClassDTO>> GetAllCourseClassesAsync()

        {
            var classes = await _context.CourseClasses

                .Include(cc => cc.Course)

                .Include(cc => cc.Enrollments)

                // thêm Teacher
                .Include(cc => cc.Teacher)
                    .ThenInclude(t => t.User)

                .ToListAsync();

            return classes.Select(cc =>
            {
                var currentStudents =
                    cc.Enrollments.Count(sc =>
                        sc.Status != EnrollmentStatus.Cancelled);

                return new CourseClassDTO
                {
                    Id = cc.Id,

                    CourseId = cc.CourseId,

                    CourseName = cc.Course.Name,
                    // teacher 
                    TeacherId = cc.TeacherId,
                    TeacherName = cc.Teacher?.Name ?? "Chưa phân công",

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
                        currentStudents >= cc.MaxStudents ,
                   
                };
            });
        }

        public async Task<CourseClassDTO> UpdateCourseClassAsync(int id, UpdateCourseClassDTO dto)
        {
            // =========================
            // LOAD COURSE CLASS
            // =========================
            var courseClass = await _context.CourseClasses

                .Include(cc => cc.Course)

                .Include(cc => cc.Enrollments)

                .Include(cc => cc.Teacher)
                    .ThenInclude(t => t.User)

                .FirstOrDefaultAsync(cc => cc.Id == id);

            if (courseClass == null)
                throw new NotFoundException("Class không tồn tại");

            // =========================
            // VALIDATE TIME
            // =========================
            if (dto.StartTime >= dto.EndTime)
                throw new BadRequestException(
                    "StartTime phải nhỏ hơn EndTime");

            // =========================
            // VALIDATE DATE
            // =========================
            if (dto.StartDate > dto.EndDate)
                throw new BadRequestException(
                    "StartDate không hợp lệ");

            // =========================
            // VALIDATE MAX STUDENTS
            // =========================
            if (dto.MaxStudents <= 0)
                throw new BadRequestException(
                    "MaxStudents phải lớn hơn 0");

            // =========================
            // VALIDATE TEACHER
            // =========================
            if (dto.TeacherId.HasValue)
            {
                var teacher =
                    await _context.Teachers
                    .FirstOrDefaultAsync(t =>
                        t.Id == dto.TeacherId.Value);

                if (teacher == null)
                    throw new NotFoundException(
                        "Teacher không tồn tại");

                if (!teacher.IsActive)
                    throw new BadRequestException(
                        "Teacher đang inactive");
                // assign FK
                courseClass.TeacherId = teacher.Id;
            }
            // =========================
            // UPDATE ENTITY
            // =========================
            courseClass.ClassName = dto.ClassName;

            courseClass.DayOfWeek = dto.DayOfWeek;

            courseClass.StartDate = dto.StartDate;

            courseClass.EndDate = dto.EndDate;

            courseClass.StartTime = dto.StartTime;

            courseClass.EndTime = dto.EndTime;

            courseClass.Session = dto.Session;

            courseClass.MaxStudents = dto.MaxStudents;

            // =========================
            // SAVE
            // =========================
            await _context.SaveChangesAsync();
            // =========================
            // RELOAD AGGREGATE
            // =========================

            var result =
                await _context.CourseClasses

                .Include(cc => cc.Course)

                .Include(cc => cc.Teacher)
                    .ThenInclude(t => t.User)

                .Include(cc => cc.Enrollments)

                .FirstAsync(cc =>
                    cc.Id == id);

            // =========================
            // CALCULATE BUSINESS DATA
            // =========================
            var currentStudents = result.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled);
            // =========================
            // RETURN DTO
            // =========================

            return new CourseClassDTO
            {
                Id = result.Id,

                CourseId = result.CourseId,

                CourseName = result.Course.Name,

                TeacherId = result.TeacherId,

                TeacherName = result.Teacher?.Name ?? "Chưa phân công",

                ClassName = result.ClassName,

                Price = result.Course.Price,

                DayOfWeek = result.DayOfWeek,

                StartDate = result.StartDate,

                EndDate = result.EndDate,

                StartTime = result.StartTime,

                EndTime = result.EndTime,

                Session = result.Session,

                MaxStudents = result.MaxStudents,

                CurrentStudents = currentStudents,

                RemainingSlots = result.MaxStudents - currentStudents,

                IsFull = currentStudents >= result.MaxStudents
            };
        }
        public async Task<bool> DeleteCourseClassAsync(int id)
        {
            var courseClass = await _context.CourseClasses
                .Include(cc => cc.Enrollments)
                .FirstOrDefaultAsync(cc =>
                    cc.Id == id);

            if (courseClass == null) throw new NotFoundException("Class không tồn tại");

            var hasStudents = courseClass.Enrollments.Any(e => e.Status != EnrollmentStatus.Cancelled);

            if (hasStudents)
                throw new BadRequestException(
                    "Không thể xóa lớp đã có học viên");

            _context.CourseClasses.Remove(courseClass);

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task AssignTeacherAsync(int classId, AssignTeacherDTO dto)
        {
            // ================= CLASS =================

            var courseClass =
                await _context.CourseClasses
                .FirstOrDefaultAsync(
                    c => c.Id == classId);

            if (courseClass == null)
            {
                throw new NotFoundException(
                    "Class không tồn tại");
            }

            // ================= TEACHER =================
            var teacher =
                await _context.Teachers
                .FirstOrDefaultAsync(
                    t => t.Id == dto.TeacherId);

            if (teacher == null)
            {
                throw new NotFoundException(
                    "Teacher không tồn tại");
            }
            // ================= STATUS =================
            if (!teacher.IsActive)
            {
                throw new BadRequestException(
                    "Teacher đã nghỉ dạy");
            }
            // ================= ASSIGN =================
            courseClass.TeacherId = teacher.Id;
            await _context.SaveChangesAsync();

        }
        public async Task<AdminClassDetailDTO> GetClassDetailAsync(int classId)
        { 
            // =========================
            // GET CLASS
            // =========================
            var courseClass = await _context.CourseClasses
                // lấy Course
                .Include(cc => cc.Course)

                // lấy Teacher
                .Include(cc => cc.Teacher)
                    .ThenInclude(t => t.User)

                // lấy Enrollment
                .Include(cc => cc.Enrollments)
                    .ThenInclude(e => e.Student)

                // lấy Payment
                .Include(cc => cc.Enrollments)
                    .ThenInclude(e => e.PaymentItems)
                        .ThenInclude(pi => pi.Payment)

                .FirstOrDefaultAsync(
                    cc => cc.Id == classId
                );

            if (courseClass == null) throw new NotFoundException("Class không tồn tại");

            // =========================
            // CURRENT STUDENTS
            // =========================
            var currentStudents = courseClass.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled);

            // =========================
            // RETURN DTO
            // =========================
            return new AdminClassDetailDTO
            {
                ClassId = courseClass.Id,

                ClassName = courseClass.ClassName,

                CourseName = courseClass.Course.Name,

                TeacherName = courseClass.Teacher != null ? courseClass.Teacher.Name : "Chưa phân công",

                DayOfWeek = courseClass.DayOfWeek,

                StartTime = courseClass.StartTime,

                EndTime = courseClass.EndTime,

                MaxStudents = courseClass.MaxStudents,

                CurrentStudents = currentStudents,

                Students = courseClass.Enrollments.Select(e =>
                {
                    var payment = e.PaymentItems.OrderByDescending(pi => pi.Payment.CreatedAt).FirstOrDefault();
                    return new ClassStudentDTO
                    {
                        StudentId = e.StudentId,

                        StudentName = e.Student.Name,

                        Username = e.Student.User.Username,

                        EnrollmentStatus = e.Status.ToString(),

                        PaymentStatus = payment != null ? payment.Payment.Status.ToString() : "Pending",

                        Amount = payment != null ? payment.Price : 0
                    };
                    }).ToList()
            };

        }
    }
}