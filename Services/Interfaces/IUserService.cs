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
        Task AssignCourseAsync(string username, int courseClassId);
        Task RemoveCourseAsync(string username, int courseClassId);

        // 👉 Lấy course của chính user
        Task<IEnumerable<CourseClassDTO>> GetMyCoursesAsync(string username);
    }
}
