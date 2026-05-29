using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Helpers;
using StudentManagementAPI.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(
            IUserService userService)
        {
            _userService = userService;
        }

        // =====================================================
        // ADMIN
        // =====================================================

        // GET ALL STUDENTS
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _userService.GetAllStudentsAsync();

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy danh sách học viên thành công",
                    result
                )
            );
        }

        // GET STUDENT BY ID
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(int id)
        {
            var result =
                await _userService.GetByIdAsync(id);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy thông tin học viên thành công",
                    result
                )
            );
        }

        // =====================================================
        // USER
        // =====================================================

        // ASSIGN COURSE
        [HttpPost("assign-course")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AssignCourse(
            AssignCourseDTO dto)
        {
            var username =
                User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
                return Unauthorized(
                    new ApiResponse<object>(
                        false,
                        "Token không hợp lệ",
                        null
                    )
                );

            await _userService.AssignCourseAsync(
                username,
                dto.CourseClassId);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Đăng ký lớp học thành công",
                    null
                )
            );
        }

        // REMOVE COURSE
        [HttpDelete("remove-course/{courseClassId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> RemoveCourse(
            int courseClassId)
        {
            var username =
                User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
                return Unauthorized(
                    new ApiResponse<object>(
                        false,
                        "Token không hợp lệ",
                        null
                    )
                );

            await _userService.RemoveCourseAsync(
                username,
                courseClassId);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Hủy đăng ký lớp học thành công",
                    null
                )
            );
        }

        // GET MY COURSES
        [HttpGet("my-courses")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyCourses()
        {
            var username =
                User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
                return Unauthorized(
                    new ApiResponse<object>(
                        false,
                        "Token không hợp lệ",
                        null
                    )
                );

            var result =
                await _userService.GetMyCoursesAsync(
                    username);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy danh sách khóa học thành công",
                    result
                )
            );
        }
    }
}