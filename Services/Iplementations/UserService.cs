using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Models;
using StudentManagementAPI.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagementAPI.Services.Iplementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // 👉 ADMIN: lấy tất cả student
        public async Task<IEnumerable<UserDTO>> GetAllStudentsAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "User")
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Age = u.Age,
                    Courses = u.StudentCourses
                        .Select(sc => sc.Course.Name)
                        .ToList()
                })
                .ToListAsync();
        }

        // 👉 ADMIN: lấy theo ID
        public async Task<UserDTO> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.StudentCourses)
                .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;

            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Age = user.Age,
                Courses = user.StudentCourses
                    .Select(sc => sc.Course.Name)
                    .ToList()
            };
        }

        // 👉 USER: chọn course (JWT)
        public async Task AssignCourseAsync(string username, int courseId)
        {
            // 1. lấy user từ token
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                throw new System.Exception("User không tồn tại");

            // 2. check course tồn tại
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                throw new System.Exception("Course không tồn tại");

            // 3. check đã chọn chưa
            var exists = await _context.StudentCourses
                .AnyAsync(x => x.UserId == user.Id && x.CourseId == courseId);

            if (exists)
                throw new System.Exception("Bạn đã chọn course này rồi");

            // 4. thêm
            _context.StudentCourses.Add(new StudentCourse
            {
                UserId = user.Id,
                CourseId = courseId
            });

            await _context.SaveChangesAsync();
        }

        // 👉 USER: bỏ course
        public async Task RemoveCourseAsync(string username, int courseId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                throw new System.Exception("User không tồn tại");

            var sc = await _context.StudentCourses
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.CourseId == courseId);

            if (sc == null)
                throw new System.Exception("Bạn chưa đăng ký course này");

            _context.StudentCourses.Remove(sc);
            await _context.SaveChangesAsync();
        }

        // 👉 USER: lấy course của chính mình
        public async Task<IEnumerable<CourseDTO>> GetMyCoursesAsync(string username)
        {
            var user = await _context.Users
                .Include(u => u.StudentCourses)
                .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                throw new System.Exception("User không tồn tại");

            return user.StudentCourses
                .Select(sc => new CourseDTO
                {
                    Id = sc.Course.Id,
                    Name = sc.Course.Name
                })
                .ToList();
        }
    }
}
