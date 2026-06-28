using System;

namespace Core.Models
{
    public class PaymentRecord
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string ReceiptNumber { get; set; } = string.Empty;
        public string? PaymentUrl { get; set; }
        public string? CustomerEmail { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
