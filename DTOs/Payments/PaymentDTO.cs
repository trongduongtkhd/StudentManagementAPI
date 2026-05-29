using StudentManagementAPI.Enums;
using System;
using System.Collections.Generic;

namespace StudentManagementAPI.DTOs.Payments
{
    public class PaymentDTO
    {
        public int Id { get; set; }

        public decimal TotalAmount { get; set; }

        public string Username { get; set; }
        public PaymentStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<PaymentItemDTO> Items { get; set; }
    }
}
