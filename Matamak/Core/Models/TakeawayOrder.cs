using System;

namespace Core.Models
{
    public class TakeawayOrder : Order
    {
        public string CustomerName { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
    }
}
