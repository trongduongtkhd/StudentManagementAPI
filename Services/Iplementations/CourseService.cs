using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Models;
using StudentManagementAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagementAPI.Services.Iplementations
{

    public class CourseService : ICourseService
    {
        private readonly AppDbContext _context;

        public CourseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CourseDTO>> GetAllAsync()
        { 
            return await _context.Courses
                .Select(c => new CourseDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    TotalStudents = c.StudentCourses.Count()
                })
                .ToListAsync();
        }

        public async Task<CourseDTO> GetByIdAsync(int id)
        {
            var course = await _context.Courses
                .Include(c => c.StudentCourses)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return null;

            return new CourseDTO
            {
                Id = course.Id,
                Name = course.Name,
                TotalStudents = course.StudentCourses.Count
            };
        }

        public async Task<CourseDTO> CreateAsync(CreateCourseDTO dto)
        {
            var exists = await _context.Courses
       .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                throw new Exception("Course đã tồn tại");

            var course = new Course
            {
                Name = dto.Name
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return new CourseDTO
            {
                Id = course.Id,
                Name = course.Name,
                TotalStudents = 0
            };
        }

        public async Task<CourseDTO> UpdateAsync(int id, UpdateCourseDTO dto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return null;

            // 👉 Check trùng (trừ chính nó)
            var exists = await _context.Courses
                .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower() && c.Id != id);

            if (exists)
                throw new Exception("Course đã tồn tại");

            course.Name = dto.Name;

            await _context.SaveChangesAsync();

            return new CourseDTO
            {
                Id = course.Id,
                Name = course.Name,
                TotalStudents = _context.StudentCourses.Count(sc => sc.CourseId == id)
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null) return false;

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
