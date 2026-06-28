using Core.DTO;
using Core.IServices;
using Core.Models;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Services
{
    public class TakeAwayOredrServices : ITakeAwayOrderService
    {
        private readonly IOrderItemsService orderItemsService;
        private readonly DataContext dataContext;

        public TakeAwayOredrServices(IOrderItemsService orderItemsService, DataContext dataContext)
        {
            this.orderItemsService = orderItemsService;
            this.dataContext = dataContext;
        }

        Order ITakeAwayOrderService.AddTakeAwayOrder(TakeAwayD order)
        {
            var takeAwayOrder = new TakeawayOrder
            {
                orderNumber = order.orderNumber ?? GetNextOrderNumber(),
                OrderDate = order.OrderDate ?? DateTime.UtcNow,
                TotalPrice = order.TotalPrice,
                CustomerName = order.CustomerName,
                Status = "Preparing"   // Default status: under preparation
            };

            foreach (var item in order.Items)
            {
                takeAwayOrder.Items.Add(orderItemsService.AddOrderItem(item));
            }

            return takeAwayOrder;
        }

        List<TakeAwayD> ITakeAwayOrderService.GetAllTakeAwayOrders()
        {
            return dataContext.TakeawayOrders.Select(order => new TakeAwayD
            {
                orderNumber = order.orderNumber,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                CustomerName = order.CustomerName
            }).ToList();
        }

        TakeAwayD ITakeAwayOrderService.GetTakeAwayOrder(int orderNumber)
        {
            var order = dataContext.TakeawayOrders.FirstOrDefault(o => o.orderNumber == orderNumber);
            if (order == null)
            {
                throw new Exception("Takeaway order not found.");
            }

            return new TakeAwayD
            {
                orderNumber = order.orderNumber,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                CustomerName = order.CustomerName,
                Items = order.Items.Select(item => new OrderItemsD
                {
                    Name = item.Name,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice,
                    PriceForOne = item.PriceForOne,
                    Note = item.Note
                }).ToList()
            };
        }

        Order ITakeAwayOrderService.UpdateTakeAwayOrder(TakeAwayD order, int orderNumber)
        {
            var existingOrder = dataContext.TakeawayOrders.FirstOrDefault(o => o.orderNumber == orderNumber);
            if (existingOrder != null)
            {
                existingOrder.OrderDate = order.OrderDate ?? existingOrder.OrderDate;
                existingOrder.TotalPrice = order.TotalPrice;
                existingOrder.CustomerName = order.CustomerName;
                return existingOrder;
            }

            throw new Exception("Takeaway order not found.");
        }

        private int GetNextOrderNumber()
        {
            return !dataContext.TakeawayOrders.Any() ? 1 : dataContext.TakeawayOrders.Max(o => o.orderNumber) + 1;
        }
    }
}
