using System.Collections.Generic;

namespace StudentManagementAPI.DTOs
{
    public class StudentDTO
    {
        public int Id { get; set; }
        public string StudentCode { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
