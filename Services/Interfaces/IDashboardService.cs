using StudentManagementAPI.DTOs;
using StudentManagementAPI.DTOs.Dashboard;
using System.Threading.Tasks;

namespace StudentManagementAPI.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<AdminDashboardDTO> GetAdminDashboardAsync();

        Task<UserDashboardDTO> GetUserDashboardAsync(
            string username);
    }
}
