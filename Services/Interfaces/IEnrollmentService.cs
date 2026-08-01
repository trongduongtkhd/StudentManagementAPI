using StudentManagementAPI.DTOs.Courses;
using StudentManagementAPI.DTOs.Enrollments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagementAPI.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task EnrollAsync(string username, int courseClassId);

        Task CancelAsync(string username, int courseClassId);

        Task<IEnumerable<MyEnrollmentDTO>> GetMyEnrollmentsAsync(string username);
        Task<IEnumerable<AdminEnrollmentDTO>>GetAllEnrollmentsAsync(string status = null);

        Task<AdminEnrollmentDetailDTO> GetEnrollmentDetailAsync(int id);

    }
}
