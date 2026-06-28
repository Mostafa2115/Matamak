using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.DTO
{
    public class OrderItemsD
    {

        [Required]
        public string Name { get; set; }=string.Empty;
        [Required]
        public decimal PriceForOne { get; set; }
        [Required]
        public int Quantity { get; set; }
        public string? Note { get; set; }
        [Required]
        public decimal TotalPrice { get; set; }
       
    }
}
