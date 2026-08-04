using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.DTOs.Teachers;
using StudentManagementAPI.Helpers;
using StudentManagementAPI.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;
        public TeachersController(
            ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        // =========================
        // GET ALL TEACHERS
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _teacherService.GetAllAsync();
            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy danh sách giáo viên thành công",
                    result
                )
            );
        }
        // =========================
        // GET BY ID
        // =========================
        [Authorize(Roles = "Admin")]    
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _teacherService.GetByIdAsync(id);
            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy thông tin giáo viên thành công",
                    result
                )
            );
        }
        // =========================
        // CREATE
        // =========================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateTeacherDTO dto)
        {
          var result = await _teacherService.CreateAsync(dto);
            return Ok(
                new ApiResponse<object>(
                    true,
                    "Tạo giáo viên thành công",
                    result
                )
            );
        }
        // =========================
        // UPDATE
        // =========================
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateTeacherDTO dto)
        {
            await _teacherService.UpdateAsync(id, dto);
            return Ok(
                new ApiResponse<object>(
                    true,
                    "Cập nhật giáo viên thành công",
                    null
                )
            );
        }
        // =========================
        // DEACTIVATE
        // =========================
        [HttpPut("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(
            int id)
        {
            await _teacherService.DeactivateAsync(id);
            return Ok(
                new ApiResponse<object>(
                    true,
                    "Vô hiệu hóa giáo viên thành công",
                    null
                )
            );
        }
        // 
        [Authorize(Roles = "Teacher")]
        [HttpGet("me/dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            var result = await _teacherService.GetDashboardAsync(username);
            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy dashboard thành công",
                    result
                )
            );
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("me/classes")]
        public async Task<IActionResult> GetMyClasses()
        {

            var username = User.FindFirstValue(ClaimTypes.Name);
            var result = await _teacherService.GetMyClassesAsync(username);
            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy danh sách lớp thành công",
                    result
                )
            );
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("me/classes/{classId}/students")]  public async Task<IActionResult> GetStudents(int classId)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            var result = await _teacherService.GetStudentsInClassAsync(username, classId);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy danh sách sinh viên thành công",
                    result
                ));
        }

        [Authorize(Roles = "Teacher")]
        [HttpPut("me/classes/{classId}/schedule")]
        public async Task<IActionResult> UpdateSchedule(int classId, UpdateScheduleDTO dto)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            await _teacherService.UpdateScheduleAsync(username, classId, dto);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Cập nhật lịch học thành công",
                    null
                )
            );
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("me/classes/{classId}/students/{studentId}")]
        public async Task<IActionResult> GetEnrollmentDetail(int classId, int studentId)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            var result = await _teacherService.GetEnrollmentDetailAsync(username, classId, studentId);

            return Ok(
                new ApiResponse<object>(
                true,
                "Lấy thông tin đăng ký thành công",
                result));
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("me/profile")]
        public async Task<IActionResult> GetProfile()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            var result = await _teacherService.GetProfileAsync(username);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy profile giáo viên thành công",
                    result
                ));
        }

        [Authorize(Roles = "Teacher")]
        [HttpPut("me/profile")]
        public async Task<IActionResult> UpdateProfile(UpdateTeacherProfileDTO dto)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            await _teacherService.UpdateProfileAsync(username, dto);
            return Ok(
                new ApiResponse<object>(
                    true,
                    "Cập nhật profile thành công",
                    null
                )
            );
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("me/schedule")]
        public async Task<IActionResult> GetMySchedule()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            var result = await _teacherService.GetMyScheduleAsync(username);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy lịch dạy thành công",
                    result));
        }


    }
}
