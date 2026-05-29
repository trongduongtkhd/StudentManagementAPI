using System.Collections.Generic;

namespace StudentManagementAPI.DTOs.Payments
{
    public class CreatePaymentDTO
    {
        public List<int> CourseClassIds { get; set; }
    }
}
