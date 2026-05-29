namespace StudentManagementAPI.DTOs
{
    public class CourseDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int TotalStudents { get; set; }

        public int TotalClasses { get; set; }
    }
}
