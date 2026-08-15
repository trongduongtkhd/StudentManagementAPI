using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(
            IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // ================= ADMIN =================

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var data =
                await _dashboardService
                    .GetAdminDashboardAsync();

            return Ok(data);
        }

        // ================= USER =================

        [HttpGet("student")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetUserDashboard()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            var data = await _dashboardService.GetUserDashboardAsync(username);
            return Ok(data);
        }
    }
}