using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs.Courses;
using StudentManagementAPI.DTOs.Enrollments;
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
    public class EnrollmentService : IEnrollmentService
    {

        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public EnrollmentService(AppDbContext context , INotificationService notificationService )
        {
            _context = context;
            _notificationService = notificationService;
        }
        public async Task EnrollAsync(string username, int courseClassId)
        {
            // ================= USER =================
            var user = await _context.Users
                .Include(u => u.Enrollments)
                    .ThenInclude(sc => sc.CourseClass)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                throw new NotFoundException("User không tồn tại");

            // ================= CLASS =================
            var selectedClass = await _context.CourseClasses
                .Include(cc => cc.Course)
                .FirstOrDefaultAsync(cc => cc.Id == courseClassId);

            if (selectedClass == null)
                throw new NotFoundException("Lớp học không tồn tại");

            // ================= CHECK CLASS FULL =================
            var currentStudents = await _context.Enrollments
                .CountAsync(sc =>
                    sc.CourseClassId == courseClassId &&
                    sc.Status != EnrollmentStatus.Cancelled);

            if (currentStudents >= selectedClass.MaxStudents)
                throw new BadRequestException("Lớp học đã đầy");

            // ================= CHECK ALREADY ENROLLED =================
            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(sc =>
                    sc.UserId == user.Id &&
                    sc.CourseClassId == courseClassId &&
                    sc.Status != EnrollmentStatus.Cancelled);

            if (alreadyEnrolled)
                throw new BadRequestException("Bạn đã đăng ký lớp này");

            // ================= CHECK SAME COURSE =================
            var sameCourse = user.Enrollments
                .Any(sc =>
                    sc.CourseClass.CourseId == selectedClass.CourseId &&
                    sc.Status != EnrollmentStatus.Cancelled);

            if (sameCourse)
                throw new BadRequestException("Bạn đã đăng ký khóa học này rồi");

            // ================= CHECK SCHEDULE CONFLICT =================
            var hasConflict = user.Enrollments.Any(sc =>

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
                throw new BadRequestException("Lịch học bị trùng giờ");

            // ================= ENROLL =================
            var studentCourse = new Enrollment
            {
                UserId = user.Id,

                CourseClassId = courseClassId,
                EnrolledAt = DateTime.UtcNow,
                Status = EnrollmentStatus.Pending,


            };

            _context.Enrollments.Add(studentCourse);

            await _context.SaveChangesAsync();
            // ================= NOTIFICATION =================
            await _notificationService.CreateAsync(
            user.Id,

            "Đăng ký khóa học thành công",

             $"Bạn đã đăng ký lớp {selectedClass.ClassName} " +
             $"thuộc khóa {selectedClass.Course.Name}"

             );
        }

        public async Task CancelAsync(string username, int courseClassId)
        {
            // ================= USER =================
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) throw new NotFoundException("User không tồn tại");

            // ================= ENROLLMENT =================

            var enrollment = await _context.Enrollments

                .Include(e => e.CourseClass)
                  .ThenInclude(cc => cc.Course)
                   
                 .FirstOrDefaultAsync(e => e.UserId == user.Id && e.CourseClassId == courseClassId);

            if (enrollment == null) throw new NotFoundException("Bạn chưa đăng ký lớp này");

            // ================= STATUS =================

            if (enrollment.Status == EnrollmentStatus.Cancelled)
            {
                throw new BadRequestException("Đăng ký đã được hủy");
            }
            // ================= CANCEL =================
            enrollment.Status = EnrollmentStatus.Cancelled;

            await _context.SaveChangesAsync();
            // ================= NOTIFICATION =================

            await _notificationService.CreateAsync(

                user.Id,

               "Hủy đăng ký",

                $"Bạn đã hủy lớp {enrollment.CourseClass.ClassName}"

             );
        }
    
        public async Task<IEnumerable<MyEnrollmentDTO>> GetMyEnrollmentsAsync(string username)
        {

            // ================= USER =================
            var user = await _context.Users
                .Include(u => u.Enrollments)
                    .ThenInclude(e => e.CourseClass)
                        .ThenInclude(cc => cc.Course)
                .Include(u => u.Enrollments)
                    .ThenInclude(e => e.PaymentItems)
                        .ThenInclude(pi => pi.Payment)
                .FirstOrDefaultAsync(
                    u => u.Username == username);

            if (user == null) throw new NotFoundException("User không tồn tại");

            // ================= RETURN =================
            return user.Enrollments.Select(e => new MyEnrollmentDTO
            {
            EnrollmentId = e.Id,

            CourseClassId = e.CourseClassId,

            CourseName = e.CourseClass.Course.Name,

            ClassName = e.CourseClass.ClassName,

            Price = e.CourseClass.Price,

            Status = e.Status.ToString(),

            EnrolledAt = e.EnrolledAt,

            PaymentStatus = e.PaymentItems.OrderByDescending(pi => pi.Payment.CreatedAt).Select(pi => pi.Payment.Status.ToString()).FirstOrDefault() ?? "Pending"

            }).ToList();
        }

        public async Task<IEnumerable<AdminEnrollmentDTO>> GetAllEnrollmentsAsync(string status = null)
        {

            var query = _context.Enrollments

                .Include(e => e.User)

                .Include(e => e.CourseClass)
                    .ThenInclude(cc => cc.Course)

                .Include(e => e.PaymentItems)
                    .ThenInclude(pi => pi.Payment)

                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(e =>
                    e.Status.ToString() == status);
            }

            return await query.Select(e => new AdminEnrollmentDTO
            {
                    Id = e.Id,

                    StudentId = e.UserId,

                    StudentName = e.User.Name,

                    Username = e.User.Username,

                    CourseId = e.CourseClass.CourseId,

                    CourseName = e.CourseClass.Course.Name,

                    CourseClassId = e.CourseClassId,

                    ClassName = e.CourseClass.ClassName,

                    EnrollmentStatus = e.Status.ToString(),

                    PaymentStatus = e.PaymentItems.OrderByDescending(pi => pi.Payment.CreatedAt).Select(pi => pi.Payment.Status.ToString()).FirstOrDefault() ?? "Pending",

                    Amount = e.PaymentItems.Select(pi => pi.Price).FirstOrDefault(),

                    EnrolledAt = e.EnrolledAt
                  }).ToListAsync();
            }

        public async Task<AdminEnrollmentDetailDTO> GetEnrollmentDetailAsync(int id)
        {

            var enrollment = await _context.Enrollments
                .Include(e => e.User)

                .Include(e => e.CourseClass)
                   .ThenInclude(cc => cc.Teacher)
                            .ThenInclude(t => t.User)

                .Include(e => e.PaymentItems)
                    .ThenInclude(pi => pi.Payment)

                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null) throw new NotFoundException("Enrollment không tồn tại");

            var paymentItem = enrollment.PaymentItems.Where(pi => pi.Payment != null).OrderByDescending(pi => pi.Payment.CreatedAt).FirstOrDefault();

            return new AdminEnrollmentDetailDTO
            {

                EnrollmentId = enrollment.Id,

                EnrollmentStatus = enrollment.Status.ToString(),

                EnrolledAt = enrollment.EnrolledAt,

                StudentId = enrollment.UserId,

                StudentName = enrollment.User.Name,

                Username = enrollment.User.Username,

                Age = enrollment.User.Age,
                DayOfWeek = enrollment.CourseClass.DayOfWeek,

                StartTime = enrollment.CourseClass.StartTime,

                EndTime = enrollment.CourseClass.EndTime,

                CourseId = enrollment.CourseClass.CourseId,

                CourseName = enrollment.CourseClass.Course.Name,
                 
                CourseClassId = enrollment.CourseClassId,

                ClassName = enrollment.CourseClass.ClassName,
                TeacherName = enrollment.CourseClass.Teacher?.User.Name,

                PaymentCode = paymentItem?.Payment.PaymentCode,

                Amount = paymentItem?.Price ?? 0,

                PaymentStatus = paymentItem?.Payment.Status.ToString() ?? "Pending",

                PaidAt = paymentItem?.Payment.PaidAt

            };
        }

    }
}
