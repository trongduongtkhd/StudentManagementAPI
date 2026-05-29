using StudentManagementAPI.DTOs.Payments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagementAPI.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDTO> CreatePaymentAsync(
            string username,
            CreatePaymentDTO dto);
        Task<bool> PayAsync(int paymentId, string username);
        Task<IEnumerable<PaymentDTO>> GetMyPaymentsAsync(string username);
        Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync();
    }

}
