using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.DTOs;
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

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // ================= ADMIN =================

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllStudentsAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user);
        }

        // ================= USER =================

        [HttpPost("assign-course")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AssignCourse(AssignCourseDTO dto)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value; 

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Token không hợp lệ");
            }

            await _userService.AssignCourseAsync(username, dto.CourseId);

            return Ok(new { message = "Đăng ký course thành công" });
        }

        [HttpDelete("remove-course/{courseId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> RemoveCourse(int courseId)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            await _userService.RemoveCourseAsync(username, courseId);

            return Ok(new { message = "Đã hủy course" });
        }
        [HttpGet("my-courses")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyCourses()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var courses = await _userService.GetMyCoursesAsync(username);

            return Ok(courses);
        }
    }
}
