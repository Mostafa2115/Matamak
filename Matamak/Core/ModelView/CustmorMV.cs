using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ModelView
{
    public class CustmorMV
    {
        public string username { get; set; }=string.Empty;
        public string Email { get; set; }= string.Empty;
        public string PhoneNumber { get; set; }= string.Empty;
        public string Address { get; set; }= string.Empty;
        public string FullName { get; set; }= string.Empty;
        public string Role { get; set; }= string.Empty;
        public List<DeliveryOrderMV> deliveryOrders { get; set; } = new List<DeliveryOrderMV>();
        public List<TakeAwayOrderMV> takeawayOrders { get; set; } = new List<TakeAwayOrderMV>();

    }
}


