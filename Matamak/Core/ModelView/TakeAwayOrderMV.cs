using System;
using System.Collections.Generic;

namespace Core.ModelView
{
    public class TakeAwayOrderMV
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public List<OrderItemsMV> Items { get; set; } = new List<OrderItemsMV>();
    }
}
