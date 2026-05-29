using StudentManagementAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagementAPI.Services.Interfaces
{
    public interface ICourseService
    {
        // COURSE
        Task<IEnumerable<CourseDTO>> GetAllAsync();

        Task<CourseDTO> GetByIdAsync(int id);

        Task<CourseDTO> CreateAsync(CreateCourseDTO dto);

        Task<CourseDTO> UpdateAsync(int id, UpdateCourseDTO dto);

        Task<bool> DeleteAsync(int id);

        

        // COURSE CLASS
        Task<CourseClassDTO> CreateCourseClassAsync(CreateCourseClassDTO dto);
        Task<IEnumerable<CourseClassDTO>> GetClassesByCourseIdAsync(int courseId);
        Task<IEnumerable<CourseClassDTO>> GetAllCourseClassesAsync();


        Task<CourseClassDTO> UpdateCourseClassAsync(int id, UpdateCourseClassDTO dto);
        Task<bool> DeleteCourseClassAsync(int id);

    }
}
