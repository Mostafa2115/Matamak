

using System.Net.NetworkInformation;

namespace Infrastructure.Services
{
    public class DineinOredrServices : IDieninOrderService
    {
        private readonly DataContext dataContext;
        private readonly IOrderItemsService orderItemsService;
        private readonly IDailyCounter dailyCounter;

        public DineinOredrServices(DataContext dataContext, IOrderItemsService orderItemsService,IDailyCounter dailyCounter)
        {
            this.dataContext = dataContext;
            this.orderItemsService = orderItemsService;
            this.dailyCounter = dailyCounter;
        }
        DineinOrder IDieninOrderService.AddDineinOrder(DineinD order)
        {
            var dineinOrder = new DineinOrder
            {
                Items = new List<OrderItems>()
            };
            dineinOrder.orderNumber = dailyCounter.DineinCount();
            dineinOrder.OrderDate = DateTime.Now;
            dineinOrder.TotalPrice = order.TotalPrice;
            dineinOrder.TableNumber = order.TableNumber;
            dineinOrder.Status = "Pending";
            foreach (var item in order.Items)
            {
                dineinOrder.Items.Add(orderItemsService.AddOrderItem(item));
            }
            return dineinOrder;
        }

        List<DineInOrderMV> IDieninOrderService.GetAllDineinOrders()
        {
            var dineinOrders = new List<DineInOrderMV>();
            var dineinOrderList = dataContext.DineinOrders.Include(o => o.Items).ToList();
            
            foreach (var order in dineinOrderList)
            {
                var dineinD = new DineInOrderMV
                {
                    Id = order.Id,
                    status = order.Status,
                    orderNumber = order.orderNumber,
                    OrderDate = order.OrderDate,
                    TotalPrice = order.TotalPrice,
                    TableNumber = order.TableNumber,
                    Items = new List<OrderItemsMV>()
                };
                foreach (var item in order.Items)
                {
                    dineinD.Items.Add(orderItemsService.GetOrderItem(item));
                }
                dineinOrders.Add(dineinD);
            }
            return dineinOrders;
        }
        DineInOrderMV IDieninOrderService.GetDineinOrder(int orderNumber)
        {
            var order = dataContext.DineinOrders.Include(o => o.Items).FirstOrDefault(o => o.Id == orderNumber);
            if (order == null)
            {
                throw new Exception("Order not found");
            }
            var dineinD = new DineInOrderMV
            {
                Id = order.Id,
                status = order.Status,
                orderNumber = order.orderNumber,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                TableNumber = order.TableNumber,
              
            };
            foreach (var item in order.Items)
            {
                var orderItemsD = orderItemsService.GetOrderItem(item);
                dineinD.Items.Add(orderItemsD);
            } return dineinD;
        }

        DineinOrder IDieninOrderService.UpdateDineinOrder(DineinD order, int orderNumber)
        {
            
            var dineinOrder = dataContext.DineinOrders.Include(o => o.Items).FirstOrDefault(o => o.Id == orderNumber || o.orderNumber == orderNumber);
            if (dineinOrder == null)
            {
                throw new Exception("Order not found");

            }
                
                dineinOrder.TotalPrice = order.TotalPrice;
                dineinOrder.TableNumber = order.TableNumber;
                dineinOrder.Items = new List<OrderItems>();
            foreach (var item in order.Items)
            {
                dineinOrder.Items.Add(orderItemsService.AddOrderItem(item));
            }

            return dineinOrder;
        }
    }
}
