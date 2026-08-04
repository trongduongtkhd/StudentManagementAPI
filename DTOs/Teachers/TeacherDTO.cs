namespace StudentManagementAPI.DTOs.Teachers
{
    public class TeacherDTO
    {
        public int Id { get; set; }
        public string TeacherCode { get; set; }
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public string Specialization { get; set; }
     
        public int YearsOfExperience { get; set; }

        public bool IsActive { get; set; }
    }
}
