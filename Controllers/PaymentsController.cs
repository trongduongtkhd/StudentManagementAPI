using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.DTOs.Payments;
using StudentManagementAPI.Helpers;
using StudentManagementAPI.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(
            IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // =====================================================
        // CREATE PAYMENT
        // =====================================================

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CreatePayment(CreatePaymentDTO dto)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(username))
                return Unauthorized(
                    new ApiResponse<object>(
                        false,
                        "Token không hợp lệ",
                        null
                    )
                );
            var result = await _paymentService.CreatePaymentAsync(username, dto);
            return Ok(
                new ApiResponse<object>(
                    true,
                    "Tạo payment thành công",
                    result
                )
            );
        }

        // =====================================================
        // PAY
        // =====================================================

        [HttpPost("{id}/pay")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Pay(int id)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(username))
                return Unauthorized(
                    new ApiResponse<object>(
                        false,
                        "Token không hợp lệ",
                        null
                    )
                );

            var result = await _paymentService.PayAsync(id, username);
            return Ok(
                new ApiResponse<object>(
                    true,
                    "Thanh toán thành công",
                    result
                )
            );
        }
        // =====================================================
        // GET MY PAYMENTS
        // =====================================================

        [HttpGet("my-payments")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyPayments()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);


            if (string.IsNullOrEmpty(username))
                return Unauthorized(
                    new ApiResponse<object>(
                        false,
                        "Token không hợp lệ",
                        null
                    )
                );
             
            var result = await _paymentService.GetMyPaymentsAsync(username);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy payments thành công",
                    result
                )
            );
        }
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPayments()
        {
            var result =
                await _paymentService.GetAllPaymentsAsync();

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Lấy tất cả payments thành công",
                    result
                )
            );
        }
    }
}