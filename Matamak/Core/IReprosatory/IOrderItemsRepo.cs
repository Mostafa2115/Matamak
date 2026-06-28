using Core.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.IReprosatory
{
    public interface IOrderItemsRepo
    {
        public void AddOrderItem(OrderItemsD orderItem);
        public void RemoveOrderItem(int orderItemId);
        public void UpdateOrderItem(OrderItemsD orderItem, int orderItemId);
        public OrderItemsD GetOrderItem(int orderItemId);

    }
}
