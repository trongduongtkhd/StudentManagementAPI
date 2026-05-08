using StudentManagementAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagementAPI.Services.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDTO>> GetAllAsync();
        Task<CourseDTO> GetByIdAsync(int id);

        Task<CourseDTO> CreateAsync(CreateCourseDTO dto);
        Task<CourseDTO> UpdateAsync(int id, UpdateCourseDTO dto);

        Task<bool> DeleteAsync(int id);
    }
}
