using System.Collections.Generic;

namespace StudentManagementAPI.DTOs
{
    public class DashboardDTO
    {
        public int TotalStudents { get; set; }
        public int TotalCourses { get; set; }
        public int TotalEnrollments { get; set; }

        public double AvgCoursesPerStudent { get; set; }

        public List<CourseStatDTO> TopCourses { get; set; }
        public List<CourseStatDTO> CourseStats { get; set; }
    }
}
