using Core.DTO;
using Core.IReprosatory;
using Core.IServices;
using Core.ModelView;
using Infrastructure.Context;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows.Markup;

namespace Infrastructure.Reprosatory
{
    public class DelivaryOrderRepo : IDeliveryOrderRepo
    {
        private readonly IDelivaryOrderService delivaryOrderService;
        private readonly IHubContext<OrderHub> hubContext;
        private readonly IOrderItemsService orderItemsService;
        private readonly DataContext dataContext;

        public DelivaryOrderRepo(IDelivaryOrderService delivaryOrderService,IHubContext<OrderHub> hubContext ,IOrderItemsService orderItemsService, DataContext dataContext)
        {
            this.delivaryOrderService = delivaryOrderService;
            this.hubContext = hubContext;
            this.orderItemsService = orderItemsService;
            this.dataContext = dataContext;
        }
        public async Task CancelDeliveryOrder(int deliveryId)
        {
            var order = dataContext.DeliveryOrders.Find(deliveryId);
            if (order == null)
            {
                throw new Exception($"Delivery order from service is null for ID {deliveryId}.");
            }

            if (order.Status != "Delivered" && order.Status != "OutForDelivery")
            {
                order.Status = "Canceled";
                dataContext.DeliveryOrders.Update(order);
                await dataContext.SaveChangesAsync();
                await BroadcastDeliveryOrderStatusAsync(order, "تم إلغاء الطلب.");
            }
            else
            {
                throw new Exception($"Cannot cancel an order that is already delivered or out for delivery for ID {deliveryId}.");
            }
        }

        public List<DeliveryOrderMV> GetDeliveryOrderByCustomerId(string custmorusername)
        {
            var order = dataContext.DeliveryOrders
                .Include(o => o.Items)
                .Where(o => o.CustomerName == custmorusername || o.CustomerUsername == custmorusername);
            List<DeliveryOrderMV> deliveryOrdersMV = new List<DeliveryOrderMV>();

            foreach (var o in order)
            {
                var orderMV = new DeliveryOrderMV
                {
                    Id = o.Id,
                    Status = o.Status,
                    OrderDate = o.OrderDate,
                    OrderNumber = o.orderNumber,
                    TotalPrice = o.TotalPrice,
                    DeliveryAddress = o.DeliveryAddress,
                    ContactNumber = o.ContactNumber,
                    CustomerName = o.CustomerName,
                    Items = new List<OrderItemsMV>()
                };
                foreach (var item in o.Items)
                {
                    orderMV.Items.Add(orderItemsService.GetOrderItem(item));
                }
                deliveryOrdersMV.Add(orderMV);
            }

            return deliveryOrdersMV;
        }
        async Task IDeliveryOrderRepo.AddDeliveryOrder(DelivaryD order)
        {
            var result = delivaryOrderService.AddDelivaryOrder(order);

            dataContext.DeliveryOrders.Add(result);
            await dataContext.SaveChangesAsync();

            var orderView = new DeliveryOrderMV
            {
                Id = result.Id,
                Status = result.Status,
                OrderNumber = result.orderNumber,
                OrderDate = result.OrderDate,
                TotalPrice = result.TotalPrice,
                DeliveryAddress = result.DeliveryAddress,
                ContactNumber = result.ContactNumber,
                CustomerName = result.CustomerName
            };

            await hubContext.Clients.Group("Cashiers").SendAsync("ReceiveNewOrder", orderView);
        }

        List<DeliveryOrderMV> IDeliveryOrderRepo.GetAllDeliveryOrders()
        {
           var orders = delivaryOrderService.GetAllDelivaryOrders();
            if (orders == null || orders.Count == 0)
            {
                 throw new Exception("Delivery orders from service are null or empty.");
            }
            return orders;
        }

        DeliveryOrderMV IDeliveryOrderRepo.GetDeliveryOrder(int deliveryId)
        {
            var order = delivaryOrderService.GetDelivaryOrder(deliveryId);
            if (order == null)
            {
                throw new Exception($"Delivery order from service is null for ID {deliveryId}.");
            }
            return order;

        }
        async Task IDeliveryOrderRepo.HandOrderToCustmor(int deliveryId)
        {
            var order = dataContext.DeliveryOrders.Find(deliveryId);
            if (order != null && order.Status != "Pending" && order.Status != "Canceled")
            {
                order.Status = "Delivered";
                dataContext.DeliveryOrders.Update(order);
                await dataContext.SaveChangesAsync();
                await BroadcastDeliveryOrderStatusAsync(order, "تم تسليم الطلب للعميل.");
            }
            else
            {
                throw new Exception($"Cannot hand order to customer. Order not found or already delivered/canceled for ID {deliveryId}.");
            }
        }

        async Task IDeliveryOrderRepo.HandOrderToDriver(int deliveryId)
        {
            var order = dataContext.DeliveryOrders.Find(deliveryId);
            if (order != null && order.Status != "Delivered" && order.Status != "Canceled")
            {
                order.Status = "OutForDelivery";
                dataContext.DeliveryOrders.Update(order);
                await dataContext.SaveChangesAsync();
                await BroadcastDeliveryOrderStatusAsync(order, $"طلبك رقم {order.orderNumber} خرج للتوصيل.");
            }
            else
            {
                throw new Exception($"Cannot hand order to driver. Order not found or already delivered/canceled for ID {deliveryId}.");
            }
        }

        void IDeliveryOrderRepo.RemoveDeliveryOrder(int deliveryId)
        {
           var order = dataContext.DeliveryOrders.Find(deliveryId);
           if (order != null)
           {
                foreach (var item in dataContext.OrderItems)
                {
                    orderItemsService.DeleteOrderItem(item);
                }
                dataContext.DeliveryOrders.Remove(order);
               dataContext.SaveChanges();
           }
           else
                throw new Exception($"Delivery order from service is null for ID {deliveryId}.");
        }

        void IDeliveryOrderRepo.UpdateDeliveryOrder(DelivaryD order, int deliveryId)
        {

            var existingOrder = delivaryOrderService.UpdateDelivaryOrder(order, deliveryId);
            if (existingOrder != null)
            {

                dataContext.DeliveryOrders.Update(existingOrder);
                dataContext.SaveChanges();
            } else throw new Exception($"Delivery order from service is null for ID {deliveryId}.");

        }
        private async Task BroadcastDeliveryOrderStatusAsync(DeliveryOrder order, string message)
        {
            var orderView = CreateDeliveryOrderView(order);
            await hubContext.Clients.Group("Cashiers").SendAsync("ReceiveOrderStatusChanged", orderView, message);

            if (!string.IsNullOrWhiteSpace(order.CustomerUsername))
            {
                await hubContext.Clients.Group("User_" + order.CustomerUsername).SendAsync("ReceiveOrderStatusChanged", orderView, message);
                await hubContext.Clients.Group("User_" + order.CustomerUsername).SendAsync("ReceiveOrderStatusUpdate", message);
            }
        }

        private static DeliveryOrderMV CreateDeliveryOrderView(DeliveryOrder order)
        {
            return new DeliveryOrderMV
            {
                Id = order.Id,
                Status = order.Status,
                OrderNumber = order.orderNumber,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                DeliveryAddress = order.DeliveryAddress,
                ContactNumber = order.ContactNumber,
                CustomerName = order.CustomerName
            };
        }
    }
}



