using System;

namespace Core.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public int TableNumber { get; set; }
        public int NumberOfGuests { get; set; }
        public DateTime ReservedAt { get; set; }
        public string Status { get; set; } = "Booked";
        public string? Notes { get; set; }
    }
}
