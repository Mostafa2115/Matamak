using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class DineinOrder: Order
    {
       
        public int TableNumber { get; set; }
        public decimal ServiceCharge {  get; set; }= 20;


    }
}
