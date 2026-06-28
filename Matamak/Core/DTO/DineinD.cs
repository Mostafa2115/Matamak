using Core.Models;
using Core.ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTO
{
    public class DineinD
    {
        [Required]
        public List<OrderItemsD> Items { get; set; }= new List<OrderItemsD>();
        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Table number must be greater than 0")]
        public int TableNumber { get; set; }
        public decimal ServiceCharge => 20;
    }
}
