using StudentManagementAPI.DTOs.Courses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagementAPI.Services.Interfaces
{
    public interface ICourseClasses
    {
        Task AssignTeacherAsync(int classId, int teacherId);
        Task<IEnumerable<CourseClassDTO>> GetClassesByCourseIdAsync(int courseId);
    }
}
