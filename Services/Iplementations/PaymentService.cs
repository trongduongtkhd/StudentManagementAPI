using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs.Payments;
using StudentManagementAPI.Enums;
using StudentManagementAPI.Models;
using StudentManagementAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagementAPI.Services.Iplementations
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        public PaymentService(AppDbContext context , INotificationService notificationService )
        {
            _context = context;
            _notificationService = notificationService;
        }

   
public async Task<PaymentDTO> CreatePaymentAsync(
    string username,
    CreatePaymentDTO dto)
        {
            // ================= USER =================

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                throw new Exception("User không tồn tại");

            // ================= VALIDATE INPUT =================

            if (dto.CourseClassIds == null || !dto.CourseClassIds.Any())
                throw new Exception("Danh sách lớp học không hợp lệ");

            // ================= GET ENROLLMENTS =================

            var enrollments = await _context.StudentCourses

                .Include(sc => sc.CourseClass)
                    .ThenInclude(cc => cc.Course)

                .Include(sc => sc.PaymentItems)
                    .ThenInclude(pi => pi.Payment)

                .Where(sc =>
                    sc.UserId == user.Id &&
                    dto.CourseClassIds.Contains(sc.CourseClassId) &&
                    sc.Status == EnrollmentStatus.Pending)

                .ToListAsync();

            // ================= VALIDATE ENROLLMENTS =================

            if (!enrollments.Any())
                throw new Exception("Không có enrollment pending");

            // ================= CHECK EXISTED PENDING PAYMENT =================

            var hasPendingPayment = enrollments.Any(sc =>
                sc.PaymentItems.Any(pi =>
                    pi.Payment.Status == PaymentStatus.Pending));

            if (hasPendingPayment)
                throw new Exception(
                    "Bạn đã có payment pending cho lớp học này");

            // ================= CREATE PAYMENT =================

            var payment = new Payment
            {
                UserId = user.Id,

                Status = PaymentStatus.Pending,

                CreatedAt = DateTime.UtcNow
            };

            // ================= CREATE PAYMENT ITEMS =================

            foreach (var enrollment in enrollments)
            {
                payment.PaymentItems.Add(new PaymentItem
                {
                    StudentCourseId = enrollment.Id,

                    Price = enrollment.CourseClass.Price
                });
            }

            // ================= TOTAL AMOUNT =================

            payment.TotalAmount = payment.PaymentItems
                .Sum(pi => pi.Price);

            // ================= SAVE DATABASE =================

            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            // ================= RETURN DTO =================

            return new PaymentDTO
            {
                Id = payment.Id,

                TotalAmount = payment.TotalAmount,

                Status = payment.Status,

                CreatedAt = payment.CreatedAt,

                Items = enrollments
                    .Select(e => new PaymentItemDTO
                    {
                        CourseClassId = e.CourseClassId,

                        CourseName = e.CourseClass.Course.Name,

                        ClassName = e.CourseClass.ClassName,

                        Price = e.CourseClass.Price
                    })
                    .ToList()
            };
        }


        public async Task<bool> PayAsync(
            int paymentId,
            string username)
        {
            // ================= USER =================
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                throw new Exception("User không tồn tại");

            // ================= PAYMENT =================
            var payment = await _context.Payments
                .Include(p => p.PaymentItems)
                    .ThenInclude(pi => pi.StudentCourse)
                        .ThenInclude(sc => sc.CourseClass)
                            .ThenInclude(cc => cc.Course)
                .FirstOrDefaultAsync(p =>
                    p.Id == paymentId &&
                    p.UserId == user.Id);

            if (payment == null)
                throw new Exception("Payment không tồn tại");

            // ================= CHECK STATUS =================
            if (payment.Status == PaymentStatus.Paid)
                throw new Exception("Payment đã thanh toán");

            // ================= UPDATE PAYMENT =================
            payment.Status = PaymentStatus.Paid;

            // ================= ACTIVATE ENROLLMENTS =================
            foreach (var item in payment.PaymentItems)
            {
                var enrollment = await _context.StudentCourses
                    .FirstOrDefaultAsync(sc =>
                        sc.Id == item.StudentCourseId);

                if (enrollment != null)
                {
                    // PAID
                    enrollment.Status = EnrollmentStatus.Paid;

                    // NOTIFICATION
                    await _notificationService.CreateAsync(
                        user.Id,
                        "Thanh toán thành công",
                        $"Bạn đã thanh toán thành công lớp " +
                        $"{item.StudentCourse.CourseClass.Course.Name} - " +
                        $"{item.StudentCourse.CourseClass.ClassName}"
                    );
                }
            }

            // ================= SAVE =================
            await _context.SaveChangesAsync();
            return true;
        }

public async Task<IEnumerable<PaymentDTO>> GetMyPaymentsAsync(
    string username)
        {
            // ================= USER =================

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == username);

            if (user == null)
                throw new Exception("User không tồn tại");

            // ================= GET PAYMENTS =================

            return await _context.Payments

                .Where(p => p.UserId == user.Id)

                .Include(p => p.PaymentItems)
                    .ThenInclude(pi => pi.StudentCourse)
                        .ThenInclude(sc => sc.CourseClass)
                            .ThenInclude(cc => cc.Course)

                .OrderByDescending(p => p.CreatedAt)

                .Select(p => new PaymentDTO
                {
                    Id = p.Id,

                    TotalAmount = p.TotalAmount,

                    Status = p.Status,

                    CreatedAt = p.CreatedAt,

                    Items = p.PaymentItems
                        .Select(pi => new PaymentItemDTO
                        {
                            CourseClassId =
                                pi.StudentCourse.CourseClassId,

                            CourseName =
                                pi.StudentCourse
                                    .CourseClass
                                    .Course
                                    .Name,

                            ClassName =
                                pi.StudentCourse
                                    .CourseClass
                                    .ClassName,

                            Price = pi.Price
                        })
                        .ToList()
                })

                .ToListAsync();
        }
        public async Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync()
        {
            return await _context.Payments

                .Include(p => p.User)

                .Include(p => p.PaymentItems)
                    .ThenInclude(pi => pi.StudentCourse)
                        .ThenInclude(sc => sc.CourseClass)
                            .ThenInclude(cc => cc.Course)

                .OrderByDescending(p => p.CreatedAt)

                .Select(p => new PaymentDTO
                {
                    Id = p.Id,

                    TotalAmount = p.TotalAmount,

                    Status = p.Status,

                    CreatedAt = p.CreatedAt,

                    Username = p.User.Username,

                    Items = p.PaymentItems
                        .Select(pi => new PaymentItemDTO
                        {
                            CourseClassId =
                                pi.StudentCourse.CourseClassId,

                            CourseName =
                                pi.StudentCourse
                                    .CourseClass
                                    .Course
                                    .Name,

                            ClassName =
                                pi.StudentCourse
                                    .CourseClass
                                    .ClassName,

                            Price = pi.Price
                        })
                        .ToList()
                })

                .ToListAsync();
        }

    }
}