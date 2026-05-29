using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StudentManagementAPI.DTOs;
using StudentManagementAPI.Helpers;
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

        public CourseClassesController(
            ICourseService courseService)
        {
            _courseService = courseService;
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
            await _courseService
                .DeleteCourseClassAsync(id);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Xóa lớp học thành công",
                    null
                )
            );
        }
    }
}