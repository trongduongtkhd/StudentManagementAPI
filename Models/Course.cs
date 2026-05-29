using System.Collections.Generic;

namespace StudentManagementAPI.Models
{
    public class Course
    {
        public int Id { get; set; }

        // Java / C# / Python
        public string Name { get; set; }

        // Mô tả khóa học
        public string Description { get; set; }
        public decimal Price { get; set; }
        // Ảnh khóa học
        public string ImageUrl { get; set; }

        // 1 Course có nhiều lớp học
        public ICollection<CourseClass> Classes { get; set; }
            = new List<CourseClass>();
    }
}
