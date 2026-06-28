using Core.DTO;
using Core.ModelView;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.IReprosatory
{
    public interface IPaymentRepo
    {
        Task<PaymentMV> ProcessPaymentAsync(PaymentRequestD request);
        PaymentMV ConfirmPayment(PaymentConfirmD request);
        List<PaymentMV> GetAll();
        SalesReportMV GetSalesReport(DateTime? from, DateTime? to);
    }
}
