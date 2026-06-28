using Core.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.IReprosatory
{
    public interface IDineinOrderRepo
    {
        public void AddDineinOrder(DineinD order);
        public void RemoveDineinOrder(int orderNumber);
        public void UpdateDineinOrder(DineinD order, int orderNumber);
        public DineInOrderMV GetDineinOrder(int orderNumber);
        public List<DineInOrderMV> GetAllDineinOrders();
        public void ChangeDineinOrderStatus(int orderNumber, string status);
    }
}
