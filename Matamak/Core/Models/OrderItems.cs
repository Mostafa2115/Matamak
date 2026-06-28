using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Models
{
    public class OrderItems
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal PriceForOne { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public decimal TotalPrice { get; set; }
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; }= new Order();
    }
}
