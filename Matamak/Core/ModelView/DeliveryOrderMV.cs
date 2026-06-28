using Core.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.ModelView
{
    public class DeliveryOrderMV
    {
        public List<OrderItemsMV> Items { get; set; }= new List<OrderItemsMV>();
        public int Id { get; set; }
        public string? Status { get; set; } 
        public decimal TotalPrice { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? ContactNumber { get; set; }
        public string? CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderNumber { get; set; }
        public decimal DeliveryFee => 30;
    }
}
