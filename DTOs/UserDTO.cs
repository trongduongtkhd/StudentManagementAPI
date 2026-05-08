using System.Collections.Generic;

namespace StudentManagementAPI.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public List<string> Courses { get; set; }
    }
}
