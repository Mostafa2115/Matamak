using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ModelView
{
    public class DineInOrderMV
    {
        public List<OrderItemsMV> Items { get; set; }= new List<OrderItemsMV>();
        public int Id { get; set; }
        public int orderNumber { get; set; }
        public string status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int TableNumber { get; set; }
         public decimal ServiceCharge => 20;

    }
}
