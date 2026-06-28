

using Core.ModelView;

namespace Infrastructure.Services
{
    public class OredrItemsServices : IOrderItemsService
    {
        private readonly DataContext dataContext;

        public OredrItemsServices(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void DeleteOrderItem(OrderItems orderItem)
        {
            dataContext.OrderItems.Remove(orderItem);
                dataContext.SaveChanges();
        }

        public OrderItemsMV GetOrderItem(OrderItems orderItem)
        {
            var orderItemDto = new OrderItemsMV();
           
            if (orderItem != null)
            {
                orderItemDto.Id = orderItem.Id;
                orderItemDto.Name = orderItem.Name;
                orderItemDto.PriceForOne = orderItem.PriceForOne;
                orderItemDto.Quantity = orderItem.Quantity;
                orderItemDto.Note = orderItem.Note;
                orderItemDto.TotalPrice = orderItem.TotalPrice;
            }
            return orderItemDto;
        }

        OrderItems IOrderItemsService.AddOrderItem(OrderItemsD orderItem)
        {
            var orderItemToAdd = new OrderItems
            {
                Name = orderItem.Name,
                PriceForOne = orderItem.PriceForOne,
                Quantity = orderItem.Quantity,
                Note = orderItem.Note,
                TotalPrice = orderItem.TotalPrice
            };
            return orderItemToAdd;
        }

        OrderItems IOrderItemsService.UpdateOrderItem(OrderItemsD orderItem, int orderItemId)
        {
            var orderItemToUpdate = dataContext.OrderItems.Find(orderItemId);
            if (orderItemToUpdate != null)
            {
                orderItemToUpdate.Name = orderItem.Name;
                orderItemToUpdate.PriceForOne = orderItem.PriceForOne;
                orderItemToUpdate.Quantity = orderItem.Quantity;
                orderItemToUpdate.Note = orderItem.Note;
                orderItemToUpdate.TotalPrice = orderItem.TotalPrice;
            }
            return orderItemToUpdate;
        }
    }
}
