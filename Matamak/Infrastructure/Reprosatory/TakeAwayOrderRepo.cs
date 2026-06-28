using Core.DTO;
using Core.IReprosatory;
using Core.IServices;
using Core.ModelView;
using Core.Models;
using Infrastructure.Context;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;

namespace Infrastructure.Reprosatory
{
    public class TakeAwayOrderRepo : ITakeAwayOrderRepo
    {
        private readonly ITakeAwayOrderService takeAwayOrderService;
        private readonly IOrderItemsService orderItemsService;
        private readonly DataContext dataContext;
        private readonly IHubContext<OrderHub> hubContext;

        public TakeAwayOrderRepo(
            ITakeAwayOrderService takeAwayOrderService,
            IOrderItemsService orderItemsService,
            DataContext dataContext,
            IHubContext<OrderHub> hubContext)
        {
            this.takeAwayOrderService = takeAwayOrderService;
            this.orderItemsService = orderItemsService;
            this.dataContext = dataContext;
            this.hubContext = hubContext;
        }

        public async Task AddTakeAwayOrder(TakeAwayD order)
        {
            var newOrder = (TakeawayOrder)takeAwayOrderService.AddTakeAwayOrder(order);
            dataContext.TakeawayOrders.Add(newOrder);
            await dataContext.SaveChangesAsync();
            await hubContext.Clients.Group("Cashiers").SendAsync("ReceiveNewTakeawayOrder", Map(newOrder));
        }

        public List<TakeAwayOrderMV> GetAllTakeAwayOrders()
        {
            return dataContext.TakeawayOrders
                .Include(o => o.Items)
                .Select(Map)
                .ToList();
        }

        public TakeAwayOrderMV GetTakeAwayOrder(int orderNumber)
        {
            var order = dataContext.TakeawayOrders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == orderNumber || o.orderNumber == orderNumber);

            if (order == null)
            {
                throw new Exception("Takeaway order not found.");
            }

            return Map(order);
        }

        public List<TakeAwayOrderMV> GetTakeAwayOrderByCustomerName(string customerName)
        {
            return dataContext.TakeawayOrders
                .Include(o => o.Items)
                .Where(o => o.CustomerName == customerName)
                .Select(Map)
                .ToList();
        }

        public async Task RemoveTakeAwayOrder(int orderNumber)
        {
            var order = dataContext.TakeawayOrders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == orderNumber || o.orderNumber == orderNumber);

            if (order == null)
            {
                throw new Exception("Takeaway order not found.");
            }

            var orderView = Map(order);
            dataContext.OrderItems.RemoveRange(order.Items);
            dataContext.TakeawayOrders.Remove(order);
            await dataContext.SaveChangesAsync();
            await hubContext.Clients.Group("Cashiers").SendAsync("ReceiveTakeawayOrderRemoved", orderView);

            if (!string.IsNullOrWhiteSpace(order.CustomerName))
            {
                await hubContext.Clients.Group("User_" + order.CustomerName).SendAsync("ReceiveTakeawayOrderRemoved", orderView);
            }
        }

        public void UpdateTakeAwayOrder(TakeAwayD order, int orderNumber)
        {
            var existingOrder = dataContext.TakeawayOrders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == orderNumber || o.orderNumber == orderNumber);

            if (existingOrder == null)
            {
                throw new Exception("Takeaway order not found.");
            }

            existingOrder.TotalPrice = order.TotalPrice;
            existingOrder.OrderDate = order.OrderDate ?? existingOrder.OrderDate;
            existingOrder.CustomerName = order.CustomerName;
            dataContext.OrderItems.RemoveRange(existingOrder.Items);
            existingOrder.Items = order.Items.Select(orderItemsService.AddOrderItem).ToList();

            dataContext.TakeawayOrders.Update(existingOrder);
            dataContext.SaveChanges();
        }

        public async Task ChangeTakeawayOrderStatus(int orderNumber, string status)
        {
            var order = dataContext.TakeawayOrders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == orderNumber || o.orderNumber == orderNumber);

            if (order == null)
            {
                throw new Exception("Takeaway order not found.");
            }

            order.Status = status;
            dataContext.TakeawayOrders.Update(order);
            await dataContext.SaveChangesAsync();

            var orderView = Map(order);
            string message = status == "Ready" ? $"طلب التيك أواي رقم {order.orderNumber} جاهز للاستلام!" : $"تم تسليم طلب التيك أواي رقم {order.orderNumber}.";

            await hubContext.Clients.Group("Cashiers").SendAsync("ReceiveTakeawayOrderStatusChanged", orderView, message);

            if (!string.IsNullOrWhiteSpace(order.CustomerName))
            {
                await hubContext.Clients.Group("User_" + order.CustomerName).SendAsync("ReceiveTakeawayOrderStatusChanged", orderView, message);
            }
        }

        private TakeAwayOrderMV Map(TakeawayOrder order)
        {
            return new TakeAwayOrderMV
            {
                Id = order.Id,
                OrderNumber = order.orderNumber,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                CustomerName = order.CustomerName,
                IsPaid = order.IsPaid,
                Items = order.Items.Select(orderItemsService.GetOrderItem).ToList()
            };
        }
    }
}

