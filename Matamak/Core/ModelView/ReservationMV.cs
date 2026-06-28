using System;

namespace Core.ModelView
{
    public class ReservationMV
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public int TableNumber { get; set; }
        public int NumberOfGuests { get; set; }
        public DateTime ReservedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
