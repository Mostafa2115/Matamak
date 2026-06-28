using Core.DTO;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.IServices
{
    public interface ITakeAwayOrderService
    { 
        public Order AddTakeAwayOrder(TakeAwayD order);
        public Order UpdateTakeAwayOrder(TakeAwayD order, int orderNumber);
        public TakeAwayD GetTakeAwayOrder(int orderNumber);
        public List<TakeAwayD> GetAllTakeAwayOrders();
         
    }
}
