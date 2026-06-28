using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class DeliveryOrder: Order
    {
        public enum DeliveryStatus
        {
            Pending,
            OutForDelivery,
            Delivered,
            Canceled
        }
        
        public bool IsPaid { get; set; }
        public string CustomerUsername { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal DeliveryFee { get; set; } = 30;

    }
}
