using System;

namespace Core.ModelView
{
    public class SalesReportMV
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int TotalOrders { get; set; }
        public int PaidOrders { get; set; }
        public decimal GrossRevenue { get; set; }
        public decimal NetRevenue { get; set; }
        public decimal TotalDiscounts { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrdersCount { get; set; }
    }
}
