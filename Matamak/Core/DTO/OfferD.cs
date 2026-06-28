using System;
using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class OfferD
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Code { get; set; } = string.Empty;
        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; }
        public decimal? FlatDiscountAmount { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
    }
}
