using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTO
{
    public class TakeAwayD
    {
        public int? orderNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        [Required]
        public List<OrderItemsD> Items { get; set; }= new List<OrderItemsD>();
        public decimal TotalPrice { get; set; }
        public string CustomerName { get; set; } = string.Empty;
    }
}
