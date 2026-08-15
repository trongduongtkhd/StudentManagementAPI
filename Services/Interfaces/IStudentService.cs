using StudentManagementAPI.DTOs;
using StudentManagementAPI.DTOs.Admin;
using StudentManagementAPI.DTOs.Courses;
using StudentManagementAPI.DTOs.Schedule;

using StudentManagementAPI.DTOs.Teachers;
using StudentManagementAPI.DTOs.Users;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace StudentManagementAPI.Services.Interfaces
{
    public interface IStudentService
    {
        // 👉 Admin dùng
        Task<IEnumerable<StudentDTO>> GetAllStudentsAsync();
        Task<StudentProfileDTO> GetByIdAsync(int id);
        Task<AdminStudentDetailDTO> GetStudentDetailAsync(int id);

      
        //Calendar
        Task<IEnumerable<ScheduleDTO>> GetMyScheduleAsync(string username);
        // PROFILE
        Task<StudentProfileDTO> GetProfileAsync(string username);
        Task UpdateProfileAsync(string username, UpdateStudentProfileDTO dto);
    }
}
