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
    public interface IUserService
    {
        // 👉 Admin dùng
        Task<IEnumerable<UserDTO>> GetAllStudentsAsync();
        Task<UserProfileDTO> GetByIdAsync(int id);
        Task<AdminStudentDetailDTO> GetStudentDetailAsync(int id);

        Task<IEnumerable<AvailableTeacherDTO>> GetAvailableTeachersAsync();
        Task<TeacherAccountDTO> CreateTeacherAccountAsync(CreateTeacherAccountDTO dto);
        //Calendar
        Task<IEnumerable<ScheduleDTO>> GetMyScheduleAsync(string username);


        // PROFILE
        Task<UserProfileDTO> GetProfileAsync(string username);
        Task UpdateProfileAsync(string username, UpdateUserProfileDTO dto);
    }
}
