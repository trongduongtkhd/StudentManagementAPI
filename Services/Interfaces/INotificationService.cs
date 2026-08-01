using StudentManagementAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagementAPI.Services.Interfaces
{
    public interface INotificationService
    {
        Task CreateAsync(
          int userId,
          string title,
          string message);

        Task<IEnumerable<NotificationDTO>> GetMyNotificationsAsync(
            string username);

        Task MarkAsReadAsync(
            int notificationId,
            string username);
        Task<int> GetUnreadCountAsync(string username);
    }
}
