using StudentManagementAPI.DTOs.Courses;
using StudentManagementAPI.DTOs.Teachers;
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
        Task<IEnumerable<CourseClassDTO>> GetAllCourseClassesAsync();


        Task<CourseClassDTO> UpdateCourseClassAsync(int id, UpdateCourseClassDTO dto);
        Task<bool> DeleteCourseClassAsync(int id);
        // gan giao vien cho lop hoc 
        Task AssignTeacherAsync(int classId, AssignTeacherDTO dto);

        // admin xem chi tiết class 
        Task<AdminClassDetailDTO> GetClassDetailAsync(int classId);

    }
}
