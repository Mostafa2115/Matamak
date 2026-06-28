using Core.DTO;
using Core.Models;
using Core.ModelView;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.IServices
{
    public interface IOrderItemsService
    {
        public OrderItems AddOrderItem(OrderItemsD orderItem);
        public OrderItems UpdateOrderItem(OrderItemsD orderItem, int orderItemId);
        public OrderItemsMV GetOrderItem(OrderItems orderItem);
        public void DeleteOrderItem(OrderItems orderItem);

    }
}
