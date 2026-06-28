using Core.DTO;
using Core.ModelView;
using System.Collections.Generic;

namespace Core.IReprosatory
{
    public interface ITakeAwayOrderRepo
    {
        Task AddTakeAwayOrder(TakeAwayD order);
        Task RemoveTakeAwayOrder(int orderNumber);
        void UpdateTakeAwayOrder(TakeAwayD order, int orderNumber);
        TakeAwayOrderMV GetTakeAwayOrder(int orderNumber);
        List<TakeAwayOrderMV> GetAllTakeAwayOrders();
        List<TakeAwayOrderMV> GetTakeAwayOrderByCustomerName(string customerName);
        Task ChangeTakeawayOrderStatus(int orderNumber, string status);
    }
}



