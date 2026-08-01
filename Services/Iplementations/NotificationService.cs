using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Models;
using StudentManagementAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagementAPI.Services.Iplementations
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task CreateAsync(
            int userId,
            string title,
            string message)
        {
            var notification = new Notification
            {
                UserId = userId,

                Title = title,

                Message = message,

                IsRead = false,

                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();
        }

        // GET MY NOTIFICATIONS
        public async Task<IEnumerable<NotificationDTO>>
            GetMyNotificationsAsync(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == username);

            if (user == null)
                throw new Exception("User không tồn tại");

            return await _context.Notifications
                .Where(n => n.UserId == user.Id)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDTO
                {
                    Id = n.Id,

                    Title = n.Title,

                    Message = n.Message,

                    IsRead = n.IsRead,

                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();
        }

        // MARK AS READ
        public async Task MarkAsReadAsync(
            int notificationId,
            string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == username);

            if (user == null)
                throw new Exception("User không tồn tại");

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.Id == notificationId &&
                    n.UserId == user.Id);

            if (notification == null)
                throw new Exception("Notification không tồn tại");

            notification.IsRead = true;

            await _context.SaveChangesAsync();
        }
        public async Task<int> GetUnreadCountAsync(
    string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == username);

            if (user == null)
                throw new Exception("User không tồn tại");

            return await _context.Notifications
                .CountAsync(n =>
                    n.UserId == user.Id &&
                    !n.IsRead);
        }
    }
}
