using StudentManagementAPI.DTOs.Schedule;
using StudentManagementAPI.DTOs.Teachers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagementAPI.Services.Interfaces
{
    public interface ITeacherService
    {
        Task<TeacherDTO> CreateAsync(CreateTeacherDTO dto);
        Task<IEnumerable<TeacherDTO>> GetAllAsync();
        Task<TeacherDetailDTO> GetByIdAsync(int id);

        Task UpdateAsync(int id, UpdateTeacherDTO dto);
        Task DeactivateAsync(int id);
        
        Task<TeacherDashboardDTO> GetDashboardAsync(string username);
        Task<IEnumerable<TeacherClassDTO>> GetMyClassesAsync(string username);
        Task<IEnumerable<TeacherStudentDTO>> GetStudentsInClassAsync(string username, int classId);
        Task UpdateScheduleAsync(string username, int classId, UpdateScheduleDTO dto);
        Task<EnrollmentDetailDTO> GetEnrollmentDetailAsync(string username, int classId, int studentId);

        Task<TeacherProfileDTO> GetProfileAsync(string username);

        //Calendar
        Task<IEnumerable<ScheduleDTO>> GetMyScheduleAsync(string username);
    }
}
