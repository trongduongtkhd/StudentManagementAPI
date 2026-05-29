namespace StudentManagementAPI.DTOs.Payments
{
    public class PaymentItemDTO
    {
        public int CourseClassId { get; set; }

        public string CourseName { get; set; }

        public string ClassName { get; set; }

        public decimal Price { get; set; }
    }
}
