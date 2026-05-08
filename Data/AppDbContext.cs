using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 👉 Composite key cho bảng many-to-many
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.UserId, sc.CourseId });

            // 👉 SEED ADMIN
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "$2a$11$hDet60JpHWxzsFIrLjU1bOohiLojZ03BlxvC0/9RysBDXVQPioSVq", // mk : 123456
                    Role = "Admin",
                    Name = "admin123",
                    Age = 25
                }
            );
        }
    }
}
