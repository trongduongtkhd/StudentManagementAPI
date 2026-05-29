namespace StudentManagementAPI.DTOs
{
    public class CourseStatDTO
    {
        public string CourseName { get; set; }

        // Tổng số lớp của course
        public int ClassCount { get; set; }

        // Tổng enrollments
        public int StudentCount { get; set; }
    }
}
