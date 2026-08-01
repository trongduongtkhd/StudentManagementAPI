using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseClass> CourseClasses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public DbSet<PaymentItem> PaymentItems { get; set; }

        public DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 👉 Composite key cho bảng many-to-many
            modelBuilder.Entity<Enrollment>()
                .ToTable("StudentCourses");

            // 1 Course có nhiều CourseClass
            modelBuilder.Entity<CourseClass>()
    .HasOne(cc => cc.Course)
    .WithMany(c => c.Classes)
    .HasForeignKey(cc => cc.CourseId);

            // 1 CourseClass có nhiều student đăng ký
            modelBuilder.Entity<Enrollment>()
    .HasOne(sc => sc.CourseClass)
    .WithMany(cc => cc.Enrollments)
    .HasForeignKey(sc => sc.CourseClassId);


            // q user co nhieu luot dang ki 
            modelBuilder.Entity<Enrollment>()
    .HasOne(sc => sc.User)
    .WithMany(u => u.Enrollments)
    .HasForeignKey(sc => sc.UserId);

            modelBuilder.Entity<PaymentItem>()
                .HasOne(pi => pi.Enrollment)
                .WithMany(sc => sc.PaymentItems)
                .HasForeignKey(pi => pi.EnrollmentId)
                .OnDelete(DeleteBehavior.NoAction);
            // =====================================================
            // DECIMAL CONFIG
            // =====================================================
            modelBuilder.Entity<Course>()
      .Property(c => c.Price)
      .HasPrecision(18, 2); 
            modelBuilder.Entity<CourseClass>()
                .Property(cc => cc.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PaymentItem>()
                .Property(pi => pi.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Teacher>()
    .HasOne(t => t.User)
    .WithOne(u => u.Teacher)
    .HasForeignKey<Teacher>(t => t.UserId);

            modelBuilder.Entity<CourseClass>()
                .HasOne(cc => cc.Teacher)
                .WithMany(t => t.CourseClasses)
                .HasForeignKey(cc => cc.TeacherId)
                .OnDelete(DeleteBehavior.NoAction);
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
