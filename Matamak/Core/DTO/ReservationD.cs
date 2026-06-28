using System;
using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class ReservationD
    {
        [Required]
        public string CustomerName { get; set; } = string.Empty;
        [Required]
        public string ContactNumber { get; set; } = string.Empty;
        [Range(1, int.MaxValue)]
        public int TableNumber { get; set; }
        [Range(1, int.MaxValue)]
        public int NumberOfGuests { get; set; }
        public DateTime ReservedAt { get; set; }
        public string? Notes { get; set; }
    }
}
