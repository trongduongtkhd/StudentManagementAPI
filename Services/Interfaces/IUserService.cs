using StudentManagementAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagementAPI.Services.Interfaces
{
    public interface IUserService
    {
        // 👉 Admin dùng
        Task<IEnumerable<UserDTO>> GetAllStudentsAsync();
        Task<UserDTO> GetByIdAsync(int id);

        // 👉 User dùng (JWT)
        Task AssignCourseAsync(string username, int courseId);
        Task RemoveCourseAsync(string username, int courseId);

        // 👉 Lấy course của chính user
        Task<IEnumerable<CourseDTO>> GetMyCoursesAsync(string username);
    }
}
