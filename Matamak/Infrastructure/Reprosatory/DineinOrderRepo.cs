using Core.DTO;
using Core.IReprosatory;
using Core.IServices;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infrastructure.Reprosatory
{
    public class DineinOrderRepo : IDineinOrderRepo
    {
        private readonly IDieninOrderService dineinOrderService;
        private readonly DataContext dataContext;

        public DineinOrderRepo(IDieninOrderService dineinOrderService ,DataContext dataContext)
        {
            this.dineinOrderService = dineinOrderService;
            this.dataContext = dataContext;
        }

        public void ChangeDineinOrderStatus(int orderNumber, string status)
        {
            var order = dataContext.DineinOrders.Find(orderNumber);
            if (order == null)
            {
                throw new Exception("Dine-in order not found.");
            }

            order.Status = status;
            dataContext.SaveChanges();
        }

        void IDineinOrderRepo.AddDineinOrder(DineinD order)
        {
           var result= dineinOrderService.AddDineinOrder(order);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to add the dine-in order.");
            }
            dataContext.DineinOrders.Add(result);
            dataContext.SaveChanges();
        }

        List<DineInOrderMV> IDineinOrderRepo.GetAllDineinOrders()
        {
           var result =dineinOrderService.GetAllDineinOrders();
            
            return result;
        }

        DineInOrderMV IDineinOrderRepo.GetDineinOrder(int orderNumber)
        {
          var result = dineinOrderService.GetDineinOrder(orderNumber);
            
            return result;
        }

        void IDineinOrderRepo.RemoveDineinOrder(int orderNumber)
        {
            var order = dataContext.DineinOrders.Find(orderNumber);
            if (order != null)
            {
                foreach (var item in order.Items)
                {
                    dataContext.OrderItems.Remove(item);
                }
                dataContext.DineinOrders.Remove(order);
                dataContext.SaveChanges();
            }
                else
                {
                    throw new InvalidOperationException("Order not found.");
                }
        }

        void IDineinOrderRepo.UpdateDineinOrder(DineinD order, int orderNumber)
        {
            var existingOrder = dineinOrderService.UpdateDineinOrder(order, orderNumber);
            dataContext.DineinOrders.Update(existingOrder);
            dataContext.SaveChanges();
        }
    }
}
