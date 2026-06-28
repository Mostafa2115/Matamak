using System;

namespace Core.ModelView
{
    public class PaymentMV
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ReceiptNumber { get; set; } = string.Empty;
        public string? PaymentUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
