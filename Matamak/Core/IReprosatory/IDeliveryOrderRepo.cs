using Core.DTO;
using Core.ModelView;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.IReprosatory
{
    public interface IDeliveryOrderRepo
    {
        public Task AddDeliveryOrder(DelivaryD order);
        public void RemoveDeliveryOrder(int deliveryId);
        public void UpdateDeliveryOrder(DelivaryD order, int deliveryId);
        public DeliveryOrderMV GetDeliveryOrder(int deliveryId);
        public List<DeliveryOrderMV> GetAllDeliveryOrders();
        public List<DeliveryOrderMV> GetDeliveryOrderByCustomerId(string custmorusername);
        public Task HandOrderToDriver(int deliveryId);
        public Task HandOrderToCustmor(int deliveryId);
        public Task CancelDeliveryOrder(int deliveryId);

    }
}


