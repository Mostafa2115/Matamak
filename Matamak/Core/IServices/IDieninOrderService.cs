global using Core.DTO;
global using Core.Models;
global using Core.ModelView; 
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.IServices
{
    public interface IDieninOrderService
    {
        public DineinOrder AddDineinOrder(DineinD order);
        public DineinOrder UpdateDineinOrder(DineinD order, int orderNumber);
        public DineInOrderMV GetDineinOrder(int orderNumber);
        public List<DineInOrderMV> GetAllDineinOrders();
    }
}
