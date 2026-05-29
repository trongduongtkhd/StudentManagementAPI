namespace StudentManagementAPI.Models
{
    public class PaymentItem
    {
        public int Id { get; set; }

        // PAYMENT
        public int PaymentId { get; set; }

        public Payment Payment { get; set; }
        // Enrollment 
        public int StudentCourseId { get; set; }
        public StudentCourse StudentCourse { get; set; }

        // PRICE
        public decimal Price { get; set; }
    }
}
