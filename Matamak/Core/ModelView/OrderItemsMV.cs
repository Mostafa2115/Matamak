using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.ModelView
{
    public class OrderItemsMV
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal PriceForOne { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
