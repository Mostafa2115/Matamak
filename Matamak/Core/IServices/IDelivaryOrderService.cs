using Core.DTO;
using Core.Models;
using Core.ModelView;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.IServices
{
    public interface IDelivaryOrderService
    {
            public  DeliveryOrder AddDelivaryOrder(DelivaryD order);
            public DeliveryOrder UpdateDelivaryOrder(DelivaryD order, int orderNumber);
            public DeliveryOrderMV GetDelivaryOrder(int orderNumber);
            public List<DeliveryOrderMV> GetAllDelivaryOrders();
        public Task MarkAsPaidAsync(int orderNumber);
    }
}
