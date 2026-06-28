using System;
using System.Collections.Generic;
using System.Text;

namespace Core.IServices
{
    public interface IPaymobService
    {
        Task<string> GetPaymentUrlAsync(int orderId, int amountCents, string customerEmail);
    }
}
