using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class PaymentConfirmD
    {
        [Required]
        public int PaymentId { get; set; }
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
