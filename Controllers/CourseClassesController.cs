using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.DTOs.Courses;
using StudentManagementAPI.DTOs.Teachers;
using StudentManagementAPI.Helpers;
using StudentManagementAPI.Services.Interfaces;
using StudentManagementAPI.Services.Interfaces;
using System.Threading.Tasks;

namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseClassesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ICourseClasses _courseClassService;
        public CourseClassesController(
            ICourseService courseService , ICourseClasses courseClassService)
        {
            _courseService = courseService;
            _courseClassService = courseClassService;
        }

        // =====================================================
        // GET ALL CLASSES
        // =====================================================

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _courseService
                    .GetAllCourseClassesAsync();

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy danh sách lớp học thành công",
                    result
                )
            );
        }

        // =====================================================
        // UPDATE CLASS
        // =====================================================

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            int id,
            UpdateCourseClassDTO dto)
        {
            var result =
                await _courseService
                    .UpdateCourseClassAsync(id, dto);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Cập nhật lớp học thành công",
                    result
                )
            );
        }

        // =====================================================
        // DELETE CLASS
        // =====================================================

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _courseService.DeleteCourseClassAsync(id);
            return Ok(
                new ApiResponse<object>(
                    true,
                    "Xóa lớp học thành công",
                    null
                )
            );
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/detail")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _courseService.GetClassDetailAsync(id);
            return Ok(
            new ApiResponse<object>(
            true,
            "Lấy chi tiết lớp thành công",
            result
            ));
        }

        [HttpPut("{classId}/teacher")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignTeacher(int classId, AssignTeacherDTO dto)
        {
            await _courseClassService.AssignTeacherAsync(classId, dto.TeacherId);
            return Ok(new ApiResponse<object>(
                true,
                "Gán giáo viên thành công",
                null));
        }

        [HttpGet("{id}/classes")]
        public async Task<IActionResult> GetClassesByCourse(int id)
        {
            var result = await _courseClassService.GetClassesByCourseIdAsync(id);
            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy danh sách lớp học thành công",
                    result
                )
            );
        }
    }
}