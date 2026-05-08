using StudentManagementAPI.DTOs;
using System.Threading.Tasks;

namespace StudentManagementAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);
        Task<object> LoginAsync(LoginDTO dto);
    }
}
