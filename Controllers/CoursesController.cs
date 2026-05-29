using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Models;
using StudentManagementAPI.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using StudentManagementAPI.Helpers;
namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _courseService.GetAllAsync();

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy danh sách khóa học thành công",
                    result
                )
            );
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateCourseDTO dto)
        {
            var result = await _courseService.CreateAsync(dto);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Tạo khóa học thành công",
                    result
                )
            );
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateCourseDTO dto)
        {
            var result =
        await _courseService.UpdateAsync(id, dto);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Cập nhật khóa học thành công",
                    result
                )
            );
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _courseService.DeleteAsync(id);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Xóa khóa học thành công",
                    null
                )
            );
        }
        [HttpPost("create-class")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateClass(CreateCourseClassDTO dto)
        {
            var result =
       await _courseService
           .CreateCourseClassAsync(dto);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Tạo lớp học thành công",
                    result
                )
            );
        }   

        [HttpGet("{id}/classes")]
        public async Task<IActionResult> GetClassesByCourse(int id)
        {
            var result = await _courseService
       .GetClassesByCourseIdAsync(id);

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
