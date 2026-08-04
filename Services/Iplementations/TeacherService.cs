    using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs.Schedule;
using StudentManagementAPI.DTOs.Teachers;
using StudentManagementAPI.Enums;
using StudentManagementAPI.Exceptions;
using StudentManagementAPI.Models;
using StudentManagementAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
namespace StudentManagementAPI.Services.Iplementations
{
    public class TeacherService : ITeacherService
    {
        private readonly AppDbContext _context;

        public TeacherService(AppDbContext context)
        {
            _context = context;

        }
        public async Task<TeacherDTO> CreateAsync(CreateTeacherDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {   
                if (string.IsNullOrWhiteSpace(dto.Username))
                    throw new BadRequestException("Username không được để trống");

                if (string.IsNullOrWhiteSpace(dto.Password))
                    throw new BadRequestException("Password không được để trống");

                if (dto.Password.Length < 6)
                    throw new BadRequestException("Password phải tối thiểu 6 ký tự");

                if (dto.YearsOfExperience < 0)
                    throw new BadRequestException("Số năm kinh nghiệm không hợp lệ");

                var exists = await _context.Users
                    .AnyAsync(x => x.Username == dto.Username);

                if (exists)
                    throw new BadRequestException("Username đã tồn tại");

                var user = new User
                {
                    Username = dto.Username,

                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),

                    Name = dto.Name,
                    JoinDate = DateTime.Now,

                    Role = "Teacher"
                };

                _context.Users.Add(user);

                await _context.SaveChangesAsync();

                var teacher = new Teacher
                {
                    UserId = user.Id,

                    Specialization = dto.Specialization,

                    YearsOfExperience = dto.YearsOfExperience,

                    IsActive = true,

                    CreatedAt = DateTime.UtcNow
                };

                _context.Teachers.Add(teacher);

                await _context.SaveChangesAsync();
                teacher.TeacherCode = $"GV{teacher.Id:D4}";

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return new TeacherDTO
                {
                    Id = teacher.Id,
                    TeacherCode = teacher.TeacherCode,

                    UserId = user.Id,

                    Username = user.Username,

                    Name = user.Name,

                    Specialization = teacher.Specialization,

                    YearsOfExperience = teacher.YearsOfExperience,

                    IsActive = teacher.IsActive
                };
            }
            catch (Exception ex) { 
                await transaction.RollbackAsync();
                throw;
            }
   
        }
    
        public async Task<IEnumerable<TeacherDTO>> GetAllAsync()
        {
            var teachers = await _context.Teachers
                .Include(t => t.User)
                .ToListAsync();
            return teachers.Select(t => new TeacherDTO
            {
                Id = t.Id,
                TeacherCode = t.TeacherCode,
                UserId = t.UserId,

                Username = t.User.Username,

                Name = t.User.Name,

                Specialization = t.Specialization,
               
                YearsOfExperience = t.YearsOfExperience,

                IsActive = t.IsActive

            }).ToList();
        }
        public async Task<TeacherDetailDTO> GetByIdAsync(int id)
        {
            var teacher = await _context.Teachers

              .Include(t => t.User)

             .Include(t => t.CourseClasses)
              .ThenInclude(c => c.Course)

            .Include(t => t.CourseClasses)
              .ThenInclude(c => c.Enrollments)

            .FirstOrDefaultAsync(t => t.Id == id);


            if (teacher == null)
            {
                throw new NotFoundException("Teacher không tồn tại"); 
            }


            return new TeacherDetailDTO
            {
                Id = teacher.Id,
                TeacherCode = teacher.TeacherCode,
                UserId = teacher.UserId,
                Username = teacher.User.Username,
                Name = teacher.User.Name,
                Bio = teacher.Bio,
                Specialization = teacher.Specialization,
                YearsOfExperience = teacher.YearsOfExperience,
                CreatedAt = teacher.CreatedAt,
                IsActive = teacher.IsActive,
                TotalClasses = teacher.CourseClasses.Count,  
                TotalStudents = teacher.CourseClasses.Sum(c => c.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled)),
                 
                Classes = teacher.CourseClasses.Select(c => new TeacherClassDTO
                {
                    ClassId = c.Id,

                    CourseName = c.Course.Name,

                    ClassName = c.ClassName,

                    DayOfWeek = c.DayOfWeek,

                    StartTime = c.StartTime,

                    EndTime = c.EndTime,

                    CurrentStudents = c.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled),

                    MaxStudents = c.MaxStudents

                }).ToList()
            };
        }
        public async Task UpdateAsync(int id, UpdateTeacherDTO dto)  
        {
            // ================= FIND =================

            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null)
            {
                throw new NotFoundException(
                    "Teacher không tồn tại");
            }
            // ================= VALIDATE =================
            if (dto.YearsOfExperience < 0)
            {
                throw new BadRequestException(
                    "Số năm kinh nghiệm không hợp lệ");
            }

            // ================= UPDATE =================
            teacher.Specialization = dto.Specialization;

            teacher.Bio = dto.Bio;

            teacher.YearsOfExperience = dto.YearsOfExperience;

            teacher.IsActive = dto.IsActive;
            await _context.SaveChangesAsync();
        }
        public async Task DeactivateAsync(int id)
        {
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Id == id);


            if (teacher == null)
            {
                throw new NotFoundException(
                    "Teacher không tồn tại");
            }


            if (!teacher.IsActive)
            {
                throw new BadRequestException(
                    "Teacher đã bị deactivate");
            }


            teacher.IsActive = false;


            await _context.SaveChangesAsync();
        }

        // 
        public async Task<TeacherDashboardDTO> GetDashboardAsync(string username)
        {
            var teacher = await _context.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t =>
                    t.User.Username == username);

            if (teacher == null) throw new NotFoundException("Teacher không tồn tại");

            var classes = await _context.CourseClasses
                .Include(cc => cc.Course)

                .Include(cc => cc.Enrollments)

                .Where(cc =>
                    cc.TeacherId == teacher.Id)

                .ToListAsync();

            return new TeacherDashboardDTO
            {
                TeacherName = teacher.User.Name,

                TotalClasses = classes.Count,

                TotalStudents = classes.Sum(c => c.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled)),

                Classes = classes.Select(c =>
                    new TeacherClassSummaryDTO
                    {
                        ClassId = c.Id,

                        CourseName = c.Course.Name,

                        ClassName = c.ClassName,

                        CurrentStudents = c.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled),

                        MaxStudents = c.MaxStudents
                    }).ToList()
            };
        }

        public async Task<IEnumerable<TeacherClassDTO>> GetMyClassesAsync(string username)
        {
            // ======================
            // GET TEACHER
            // ======================
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.User.Username == username);

            if (teacher == null) throw new NotFoundException("Teacher không tồn tại");
            // ======================
            // GET CLASSES
            // ======================
            var classes =
                await _context.CourseClasses

                .Include(cc => cc.Course)

                .Include(cc => cc.Enrollments)

                .Where(cc => cc.TeacherId == teacher.Id)

                .ToListAsync();
            // ======================
            // MAP DTO
            // ======================
            return classes.Select(cc =>
            {
                var students = cc.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled);
                return new TeacherClassDTO
                {
                    ClassId = cc.Id,

                    CourseId = cc.CourseId,

                    CourseName = cc.Course.Name,

                    ClassName = cc.ClassName,

                    DayOfWeek = cc.DayOfWeek,

                    StartDate = cc.StartDate,

                    EndDate = cc.EndDate,

                    StartTime = cc.StartTime,

                    EndTime = cc.EndTime,

                    CurrentStudents = students,

                    MaxStudents = cc.MaxStudents,

                    IsFull = students >= cc.MaxStudents
                };
            }).ToList();

        }
        public async Task<IEnumerable<TeacherStudentDTO>> GetStudentsInClassAsync(string username, int classId)
        {
            // ====================
            // GET TEACHER
            // ====================
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.User.Username == username);
            if (teacher == null) throw new NotFoundException("Teacher không tồn tại");
            // ====================
            // GET CLASS
            // ====================
            var courseClass = await _context.CourseClasses
                .Include(c => c.Enrollments)

                .ThenInclude(e => e.User)

                .FirstOrDefaultAsync(c => c.Id == classId && c.TeacherId == teacher.Id);

            if (courseClass == null) throw new ForbiddenException("Bạn không có quyền xem lớp này");
            // ====================
            // MAP
            // ====================
            return courseClass.Enrollments

                .Where(e => e.Status != EnrollmentStatus.Cancelled)

                .Select(e => new TeacherStudentDTO
                {

                    StudentId = e.UserId,

                    Username = e.User.Username,

                    Name = e.User.Name,

                    Status = e.Status.ToString(),

                    EnrolledAt = e.EnrolledAt
                }).ToList();
        }
        public async Task UpdateScheduleAsync(string username, int classId, UpdateScheduleDTO dto)
        {
            // ======================
            // GET TEACHER
            // ======================
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.User.Username == username);

            if (teacher == null) throw new NotFoundException("Teacher không tồn tại");

            // ======================
            // GET CLASS
            // OWNERSHIP CHECK
            // ======================

            var courseClass = await _context.CourseClasses.FirstOrDefaultAsync(c => c.Id == classId && c.TeacherId == teacher.Id);

            if (courseClass == null) throw new ForbiddenException("Bạn không có quyền sửa lớp này");

            // ======================
            // VALIDATE
            // ======================

            if (dto.StartTime >= dto.EndTime)
                throw new BadRequestException("Giờ bắt đầu phải nhỏ hơn giờ kết thúc");

            if (dto.StartDate > dto.EndDate) throw new BadRequestException("Ngày bắt đầu không hợp lệ");

            // ======================
            // UPDATE ONLY SCHEDULE
            // ======================
            courseClass.DayOfWeek = dto.DayOfWeek;

            courseClass.StartDate = dto.StartDate;

            courseClass.EndDate = dto.EndDate;

            courseClass.StartTime = dto.StartTime;

            courseClass.EndTime = dto.EndTime;

            courseClass.Session = dto.Session;

            await _context.SaveChangesAsync();
        }
        public async Task<EnrollmentDetailDTO> GetEnrollmentDetailAsync(string username, int classId, int studentId)
        {
            // ======================
            // GET TEACHER
            // ======================
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.User.Username == username);

            if (teacher == null) throw new NotFoundException("Teacher không tồn tại");

            // ======================
            // GET ENROLLMENT
            // ======================
            var enrollment = await _context.Enrollments

                .Include(e => e.User)

                .Include(e => e.CourseClass)

                 .ThenInclude(cc => cc.Teacher)

                .Include(e => e.PaymentItems)

                  .ThenInclude(pi => pi.Payment)
                .Include(e => e.CourseClass)
                  .ThenInclude(cc => cc.Course)

                .Include(e => e.CourseClass)
                  .ThenInclude(cc => cc.Teacher)

                .FirstOrDefaultAsync(e => e.UserId == studentId && e.CourseClassId == classId && e.CourseClass.TeacherId == teacher.Id);

            if (enrollment == null) throw new BadRequestException("Không tìm thấy enrollment");

            var payment = enrollment.PaymentItems.FirstOrDefault();

            return new EnrollmentDetailDTO
            {
                StudentId = enrollment.UserId,

                Username = enrollment.User.Username,

                StudentName = enrollment.User.Name,
                CourseName = enrollment.CourseClass.Course.Name,

                EnrollmentStatus = enrollment.Status.ToString(),

                EnrolledAt = enrollment.EnrolledAt,

                Amount = payment?.Price ?? 0,

                PaymentStatus = payment?.Payment.Status.ToString() ?? "Unpaid",

                PaidAt = payment?.Payment.PaidAt

            };
        }


        public async Task<TeacherProfileDTO> GetProfileAsync(string username)
        {
            var teacher = await _context.Teachers

                .Include(t => t.User)

                .FirstOrDefaultAsync(t => t.User.Username == username);

            if (teacher == null)
                throw new NotFoundException("Teacher không tồn tại");

            return new TeacherProfileDTO
            {
                UserId = teacher.UserId,

                TeacherCode = teacher.TeacherCode,

                Username = teacher.User.Username,

                Name = teacher.User.Name,

                Email = teacher.User.Email,

                Phone = teacher.User.Phone,

                Address = teacher.User.Address,

                Gender = teacher.User.Gender,

                DateOfBirth = teacher.User.DateOfBirth,

                JoinDate = teacher.User.JoinDate,

                IsActive = teacher.IsActive,

                Specialization = teacher.Specialization,

                YearsOfExperience = teacher.YearsOfExperience,

                Bio = teacher.Bio
            };
        }


        public async Task UpdateProfileAsync(string username, UpdateTeacherProfileDTO dto)
        {
            var teacher = await _context.Teachers
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.User.Username == username);

            if (teacher == null)
                throw new NotFoundException("Teacher không tồn tại");


            // Validate Email
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                try
                {
                    var email = new System.Net.Mail.MailAddress(dto.Email);

                    if (email.Address != dto.Email)
                        throw new BadRequestException("Email không hợp lệ");
                }
                catch
                {
                    throw new BadRequestException("Email không hợp lệ");
                }
            }

            // Validate Phone
            if (!string.IsNullOrWhiteSpace(dto.Phone))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Phone, @"^[0-9]{10,11}$"))
                    throw new BadRequestException("Số điện thoại không hợp lệ");
            }

            // Validate DOB
            if (dto.DateOfBirth.HasValue)
            {
                if (dto.DateOfBirth > DateTime.Today)
                    throw new BadRequestException("Ngày sinh không hợp lệ");

                var age = DateTime.Today.Year - dto.DateOfBirth.Value.Year;

                if (dto.DateOfBirth.Value.Date > DateTime.Today.AddYears(-age))
                    age--;

                if (age < 18)
                    throw new BadRequestException("Giáo viên phải từ 18 tuổi");
            }

            // Validate Gender
            if (!string.IsNullOrWhiteSpace(dto.Gender))
            {
                var genders = new[] { "Male", "Female", "Other" };

                if (!genders.Contains(dto.Gender))
                    throw new BadRequestException("Gender không hợp lệ");
            }

            // Validate Address
            if (!string.IsNullOrWhiteSpace(dto.Address))
            {
                if (dto.Address.Length > 255)
                    throw new BadRequestException("Địa chỉ tối đa 255 ký tự");
            }

            // Validate Bio
            if (!string.IsNullOrWhiteSpace(dto.Bio))
            {
                if (dto.Bio.Length > 500)
                    throw new BadRequestException("Bio tối đa 500 ký tự");
            }
            teacher.User.Email = dto.Email;
            teacher.User.Phone = dto.Phone;
            teacher.User.Address = dto.Address;
            teacher.User.Gender = dto.Gender;
            teacher.User.DateOfBirth = dto.DateOfBirth;

            teacher.Bio = dto.Bio;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ScheduleDTO>> GetMyScheduleAsync(string username)
        {
            var teacher = await _context.Teachers
    .Include(t => t.User)
    .FirstOrDefaultAsync(t => t.User.Username == username);

            if (teacher == null)
                throw new NotFoundException("Teacher không tồn tại");

            var classes = await _context.CourseClasses

                .Where(c => c.TeacherId == teacher.Id)

                .Include(c => c.Course)

                .Include(c => c.Enrollments)

                .ToListAsync();

            return classes.Select(c => new ScheduleDTO
            {
                CourseClassId = c.Id,

                CourseName = c.Course.Name,

                ClassName = c.ClassName,

                TeacherName = teacher.User.Name,

                DayOfWeek = c.DayOfWeek,

                StartTime = c.StartTime,

                EndTime = c.EndTime,

                StartDate = c.StartDate,

                EndDate = c.EndDate,

                CurrentStudents =
                    c.Enrollments.Count(e =>
                        e.Status != EnrollmentStatus.Cancelled),

                MaxStudents = c.MaxStudents
            });
        }
    }
}
