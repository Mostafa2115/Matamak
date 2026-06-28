using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTO
{
    public class DelivaryD
    {
        
        public List<OrderItemsD> Items { get; set; }= new List<OrderItemsD>();
        [Required]
        public decimal TotalPrice { get; set; }
        [Required]
        [MinLength(20)]
        public string DeliveryAddress { get; set; }=string.Empty;
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string ContactNumber { get; set; }=string.Empty;
        [Required]
        public string CustomerName { get; set; }=string.Empty;
        public decimal DeliveryFee => 30;
        
        
    }
}
