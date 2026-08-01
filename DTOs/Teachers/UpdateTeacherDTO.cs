namespace StudentManagementAPI.DTOs.Teachers
{
    public class UpdateTeacherDTO
    {
        public string Specialization { get; set; }

        public string Bio { get; set; }

        public int YearsOfExperience { get; set; }

        public bool IsActive { get; set; }
    }
}
