using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.Helpers;
using StudentManagementAPI.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET MY NOTIFICATIONS
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            var result = await _notificationService
                .GetMyNotificationsAsync(username);

            return Ok(
     new ApiResponse<object>(
         true,
         "Lấy thông báo thành công",
         result
     )
 );
        }

        // MARK AS READ
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            await _notificationService
                .MarkAsReadAsync(id, username);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Đã đánh dấu đã đọc",
                    null
                )
            );
        }
    }
}