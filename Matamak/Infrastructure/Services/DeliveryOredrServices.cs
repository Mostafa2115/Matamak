global using Core.DTO;
global using Core.Models;
global using Infrastructure.Context;
global using Core.ModelView;
global using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class DeliveryOredrServices : IDelivaryOrderService
    {
        private readonly DataContext dataContext;
        private readonly UserManager<AppUser> userManager;
        private readonly IOrderItemsService orderItemsService;
        private readonly IDailyCounter dailyCounter;

        public DeliveryOredrServices(DataContext dataContext,UserManager<AppUser> userManager, IOrderItemsService orderItemsService,IDailyCounter dailyCounter)
        {
            this.dataContext = dataContext;
            this.userManager = userManager;
            this.orderItemsService = orderItemsService;
            this.dailyCounter = dailyCounter;
        }
        DeliveryOrder IDelivaryOrderService.AddDelivaryOrder(DelivaryD order)
        {
            var newOrder = new DeliveryOrder { Items = new List<OrderItems>() };
            newOrder.orderNumber = dailyCounter.DeliveryCount();
            newOrder.Status = "Pending";
            newOrder.OrderDate = DateTime.Now;
            newOrder.TotalPrice = order.TotalPrice;
            newOrder.DeliveryAddress = order.DeliveryAddress;
            newOrder.ContactNumber = order.ContactNumber;
            newOrder.CustomerName = order.CustomerName;
            newOrder.CustomerUsername = order.CustomerName;

            foreach (var item in order.Items)
            {
                newOrder.Items.Add(orderItemsService.AddOrderItem(item));
            }
            ;
            return newOrder;
        }
        public async Task MarkAsPaidAsync(int orderNumber)
        {
            var order = await dataContext.DeliveryOrders
                .OfType<DeliveryOrder>()
                .FirstOrDefaultAsync(o => o.orderNumber == orderNumber);

            if (order != null)
            {
                order.IsPaid = true;
                dataContext.DeliveryOrders.Update(order);
                await dataContext.SaveChangesAsync();
            }
        }



        List<DeliveryOrderMV> IDelivaryOrderService.GetAllDelivaryOrders()
        {
            var orders = dataContext.DeliveryOrders.ToList();
           
            var orderDtos = new List<DeliveryOrderMV>();
            foreach (var order in orders)
            {

                var orderDto = new DeliveryOrderMV
                {
                    Id = order.Id,
                    Status = order.Status.ToString(),
                    OrderNumber = order.orderNumber,
                    OrderDate = order.OrderDate,
                    TotalPrice = order.TotalPrice,
                    DeliveryAddress = order.DeliveryAddress,
                    ContactNumber = order.ContactNumber,
                    CustomerName = order.CustomerName
                };
                orderDtos.Add(orderDto);
            }
            return orderDtos;
        }

        DeliveryOrderMV IDelivaryOrderService.GetDelivaryOrder(int orderNumber)
        {
            var order = dataContext.DeliveryOrders
            .Include(o => o.Items)
            .FirstOrDefault(o => o.Id == orderNumber);
            if (order == null)
            {
                throw new Exception("Order not found");
            }
            DeliveryOrderMV deliveryOrderMV= new DeliveryOrderMV ();
            deliveryOrderMV.Id = order.Id;
            deliveryOrderMV.Status = order.Status.ToString();
            deliveryOrderMV.OrderNumber = orderNumber;
            deliveryOrderMV.OrderDate = order.OrderDate;
            deliveryOrderMV.ContactNumber = order.ContactNumber;
            deliveryOrderMV.CustomerName = order.CustomerName;
            deliveryOrderMV.DeliveryAddress = order.DeliveryAddress;
            deliveryOrderMV.TotalPrice = order.TotalPrice;
            deliveryOrderMV.Items = new List<OrderItemsMV>();
            foreach (var item in order.Items)
            {
                deliveryOrderMV.Items.Add(orderItemsService.GetOrderItem(item));
            }


            return deliveryOrderMV;
        }

        DeliveryOrder IDelivaryOrderService.UpdateDelivaryOrder(DelivaryD order, int orderNumber)
        {
            var DeliveryOrder =dataContext.DeliveryOrders.Include(o => o.Items).FirstOrDefault(e=>e.Id == orderNumber);

            if (DeliveryOrder == null)
            {
                throw new Exception("Order not found");
            }
            DeliveryOrder.TotalPrice = order.TotalPrice;
            DeliveryOrder.DeliveryAddress = order.DeliveryAddress;
            DeliveryOrder.ContactNumber = order.ContactNumber;
            DeliveryOrder.CustomerName = order.CustomerName;
            DeliveryOrder.Items = new List<OrderItems>();

            
                foreach (var item in order.Items)
                {
                    DeliveryOrder.Items.Add(orderItemsService.AddOrderItem(item));
                }


                return DeliveryOrder;
        }   
           

        
    }
}
