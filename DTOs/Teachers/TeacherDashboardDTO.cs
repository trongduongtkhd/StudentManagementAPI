using System.Collections.Generic;

namespace StudentManagementAPI.DTOs.Teachers
{
    public class TeacherDashboardDTO
    {
        public string TeacherName { get; set; }

        public int TotalClasses { get; set; }

        public int TotalStudents { get; set; }


        public List<TeacherClassSummaryDTO> Classes { get; set; }
    }


    public class TeacherClassSummaryDTO
    {
        public int ClassId { get; set; }


        public string CourseName { get; set; }


        public string ClassName { get; set; }


        public int CurrentStudents { get; set; }


        public int MaxStudents { get; set; }
    }
}
