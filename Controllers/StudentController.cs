using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.DTOs.Courses;
using StudentManagementAPI.DTOs.Teachers;
using StudentManagementAPI.DTOs.Users;
using StudentManagementAPI.Helpers;
using StudentManagementAPI.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _userService;

        public StudentController(
            IStudentService userService)
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
            var result = await _userService.GetByIdAsync(id);
            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy thông tin học viên thành công",
                    result
                )
            );
        }

        [HttpGet("admin/students/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStudentDetail(int id)
        {
            var result = await _userService.GetStudentDetailAsync(id);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy chi tiết học viên thành công",
                    result
                )
            );
        }




      
        [Authorize(Roles = "Student")]
        [HttpGet("me/schedule")]
        public async Task<IActionResult> GetMySchedule()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            var result = await _userService.GetMyScheduleAsync(username);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy lịch học thành công",
                    result));
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            var result = await _userService.GetProfileAsync(username);

            return Ok(
                new ApiResponse<StudentProfileDTO>(
                    true,
                    "Lấy profile thành công",
                    result));
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateStudentProfileDTO dto)    
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            await _userService.UpdateProfileAsync(username, dto);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Cập nhật profile thành công",
                    null));
        }

    }
 }
