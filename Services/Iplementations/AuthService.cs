using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using StudentManagementAPI.Data;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Exceptions;
using StudentManagementAPI.Middleware;
using StudentManagementAPI.Models;
using StudentManagementAPI.Models;
using StudentManagementAPI.Services.Interfaces;
using System;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq;
using System.Security.Claims;
using System.Security.Claims;
using System.Text;
using System.Text;
using System.Threading.Tasks;
namespace StudentManagementAPI.Services.Iplementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new BadRequestException("Username không được để trống");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new BadRequestException("Password không được để trống");

            if (dto.Password.Length < 6)
                throw new BadRequestException("Password phải >= 6 ký tự");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new BadRequestException("Họ tên không được để trống");

            var existingUser = await _context.Users
                .AnyAsync(x => x.Username == dto.Username);
             
            if (existingUser) throw new BadRequestException("Username đã tồn tại");
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // =====================================================
                // 1. CREATE USER ACCOUNT
                // =====================================================
                var user = new User
                {
                    Username = dto.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = "Student"
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                // =====================================================
                // 2. CREATE STUDENT PROFILE
                // =====================================================
                var student = new Student
                {
                    UserId = user.Id,

                    Name = dto.Name,

                    JoinDate = DateTime.UtcNow,

                    IsActive = true
                };
               _context.Students.Add(student);
                await _context.SaveChangesAsync();
                // =====================================================
                // 3. GENERATE STUDENT CODE
                // =====================================================
                student.StudentCode = $"ST{student.Id:D5}";
                await _context.SaveChangesAsync();
                // =====================================================
                // 4. COMMIT
                // =====================================================
                await transaction.CommitAsync();
                return "Đăng ký thành công";
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<object> LoginAsync(LoginDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == dto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedException("Sai tài khoản hoặc mật khẩu");

            var token = GenerateToken(user);

            return new
            {
                token,
                username = user.Username,
                role = user.Role
            };
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("THIS_IS_MY_SUPER_SECRET_KEY_123456789_ABC"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
          {
            new Claim("id", user.Id.ToString()), // 🔥 QUAN TRỌNG
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
         };
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
