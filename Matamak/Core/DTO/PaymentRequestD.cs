using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class PaymentRequestD
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public string OrderType { get; set; } = string.Empty;
        [Required]
        public string PaymentMethod { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public string? OfferCode { get; set; }
    }
}
