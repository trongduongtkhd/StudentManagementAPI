using StudentManagementAPI.DTOs;
using System.Threading.Tasks;

namespace StudentManagementAPI.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDTO> GetDashboardAsync();
    }
}
