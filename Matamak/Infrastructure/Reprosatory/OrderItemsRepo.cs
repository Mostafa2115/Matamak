using Core.DTO;
using Core.IReprosatory;
using Core.IServices;
using Core.ModelView;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Reprosatory
{
    public class OrderItemsRepo : IOrderItemsRepo
    {
        private readonly DataContext dataContext;
        private readonly IOrderItemsService orderItemsService;

        public OrderItemsRepo(DataContext dataContext ,IOrderItemsService orderItemsService)
        {
            this.dataContext = dataContext;
            this.orderItemsService = orderItemsService;
        }
        public OrderItemsMV GetOrderItem(int orderItemId)
        {
           var orderItem = dataContext.OrderItems.Find(orderItemId);
            if (orderItem == null)
            {
                throw new Exception("orderItem not Found");
            }
            var result = orderItemsService.GetOrderItem(orderItem);
            return result;
        }

        void IOrderItemsRepo.AddOrderItem(OrderItemsD orderItem)
        {
           var orderItemToAdd = orderItemsService.AddOrderItem(orderItem);
            dataContext.OrderItems.Add(orderItemToAdd);
            dataContext.SaveChanges();
        }

        OrderItemsD IOrderItemsRepo.GetOrderItem(int orderItemId)
        {
            throw new NotImplementedException();
        }

        void IOrderItemsRepo.RemoveOrderItem(int orderItemId)
        {
            var orderItem = dataContext.OrderItems.Find(orderItemId);
            if (orderItem != null)
            {
                dataContext.OrderItems.Remove(orderItem);
                dataContext.SaveChanges();
            }
        }

        void IOrderItemsRepo.UpdateOrderItem(OrderItemsD orderItem, int orderItemId)
        {
            var orderItemToUpdate = orderItemsService.UpdateOrderItem(orderItem, orderItemId);
            dataContext.OrderItems.Update(orderItemToUpdate);
            dataContext.SaveChanges();
        }
    }
}
