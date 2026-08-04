namespace StudentManagementAPI.DTOs
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }

        // 👉 THÊM
        public string Name { get; set; }
        //public int Age { get; set; }
    }
}
