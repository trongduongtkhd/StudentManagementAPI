using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.Helpers;
using StudentManagementAPI.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // =================================================
        // STUDENT ENROLL CLASS
        // =================================================

        [HttpPost("{courseClassId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Enroll(int courseClassId)
        {

            var username = User.FindFirstValue(ClaimTypes.Name);

            await _enrollmentService.EnrollAsync(username, courseClassId);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Đăng ký lớp học thành công",
                    null
                )
            );
        }


        // =================================================
        // CANCEL ENROLLMENT
        // =================================================


        [HttpPut("{courseClassId}/cancel")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Cancel(int courseClassId)
        {

            var username = User.FindFirstValue(ClaimTypes.Name);

            await _enrollmentService.CancelAsync(username, courseClassId);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Hủy đăng ký thành công",
                    null
                )
            );
        }

        // =================================================
        // GET MY ENROLLMENTS
        // =================================================


        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            var result = await _enrollmentService.GetMyEnrollmentsAsync(username);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy danh sách đăng ký thành công",
                    result
                )
            );
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(string status)
        {

            var result = await _enrollmentService.GetAllEnrollmentsAsync(status);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy danh sách enrollment thành công",
                    result
                )
            );
        }

        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _enrollmentService.GetEnrollmentDetailAsync(id);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy chi tiết enrollment thành công",
                    result
                )
            );
        }

    }
}
