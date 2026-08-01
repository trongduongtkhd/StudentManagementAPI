namespace StudentManagementAPI.DTOs.Teachers
{
    public class CreateTeacherDTO
    {
        // Account
        public string Username { get; set; }

        public string Password { get; set; }

        // User
        public string Name { get; set; }

        public int Age { get; set; }

        // Teacher
        public string Specialization { get; set; }

        public string Bio { get; set; }

        public int YearsOfExperience { get; set; }
    }
}
