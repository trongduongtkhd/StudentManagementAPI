using StudentManagementAPI.Enums;
using System;
using System.Collections.Generic;

namespace StudentManagementAPI.Models
{
    public class Payment
    {
        public int Id { get; set; }

        // USER
        public int UserId { get; set; }

        public User User { get; set; }

        // TOTAL
        public decimal TotalAmount { get; set; }

        // STATUS
        public PaymentStatus Status { get; set; }

        // CREATED
        public DateTime CreatedAt { get; set; }

        // PAYMENT ITEMS
        public ICollection<PaymentItem> PaymentItems { get; set; }
            = new List<PaymentItem>();
    }
}
