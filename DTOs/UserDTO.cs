using System.Collections.Generic;

namespace StudentManagementAPI.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string StudentCode { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
