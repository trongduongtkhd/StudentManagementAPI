using System;

namespace StudentManagementAPI.DTOs.Teachers
{
    public class UpdateScheduleDTO
    {
        public DayOfWeek DayOfWeek { get; set; }


        public DateTime StartDate { get; set; }


        public DateTime EndDate { get; set; }


        public TimeSpan StartTime { get; set; }


        public TimeSpan EndTime { get; set; }


        public string Session { get; set; }
    }
}
