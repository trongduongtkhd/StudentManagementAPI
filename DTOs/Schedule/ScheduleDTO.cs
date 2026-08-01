using System;

namespace StudentManagementAPI.DTOs.Schedule
{
    public class ScheduleDTO
    {
        public int CourseClassId { get; set; }
        public int TeacherId { get; set; }
        public string CourseName { get; set; } = string.Empty;

        public string ClassName { get; set; } = string.Empty;

        public string TeacherName { get; set; } = string.Empty;

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int CurrentStudents { get; set; }

        public int MaxStudents { get; set; }
    }
}
