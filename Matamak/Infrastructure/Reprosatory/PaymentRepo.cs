using Core.DTO;
using Core.IReprosatory;
using Core.IServices;
using Core.ModelView;
using Core.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Reprosatory
{
    public class PaymentRepo : IPaymentRepo
    {
        private readonly DataContext dataContext;
        private readonly IPaymobService paymobService;

        public PaymentRepo(DataContext dataContext, IPaymobService paymobService)
        {
            this.dataContext = dataContext;
            this.paymobService = paymobService;
        }

        public PaymentMV ConfirmPayment(PaymentConfirmD request)
        {
            var payment = dataContext.PaymentRecords.FirstOrDefault(p => p.Id == request.PaymentId);
            if (payment == null)
            {
                throw new Exception("Payment not found.");
            }

            payment.Status = request.Status;
            dataContext.PaymentRecords.Update(payment);
            MarkOrderAsPaid(payment.OrderId, payment.OrderType, request.Status == "Paid");
            dataContext.SaveChanges();

            return Map(payment, 0);
        }

        public List<PaymentMV> GetAll()
        {
            return dataContext.PaymentRecords
                .OrderByDescending(p => p.CreatedAt)
                .AsEnumerable()
                .Select(p => Map(p, 0))
                .ToList();
        }

        public async Task<PaymentMV> ProcessPaymentAsync(PaymentRequestD request)
        {
            var order = GetOrder(request.OrderId, request.OrderType);
            var grossAmount = order.TotalPrice;
            var discountAmount = GetDiscountAmount(request.OfferCode, grossAmount);
            var netAmount = Math.Max(0, grossAmount - discountAmount);

            var payment = new PaymentRecord
            {
                OrderId = request.OrderId,
                OrderType = request.OrderType,
                Amount = netAmount,
                PaymentMethod = request.PaymentMethod,
                Status = request.PaymentMethod.Equals("Cash", StringComparison.OrdinalIgnoreCase) ? "Paid" : "Pending",
                ReceiptNumber = $"REC-{DateTime.UtcNow:yyyyMMddHHmmss}-{request.OrderId}",
                CustomerEmail = request.CustomerEmail
            };

            if (!request.PaymentMethod.Equals("Cash", StringComparison.OrdinalIgnoreCase))
            {
                payment.PaymentUrl = await paymobService.GetPaymentUrlAsync(request.OrderId, (int)(netAmount * 100), request.CustomerEmail ?? "customer@matamak.local");
            }

            dataContext.PaymentRecords.Add(payment);
            MarkOrderAsPaid(request.OrderId, request.OrderType, payment.Status == "Paid");
            dataContext.SaveChanges();

            return Map(payment, discountAmount);
        }

        public SalesReportMV GetSalesReport(DateTime? from, DateTime? to)
        {
            var start = from ?? DateTime.UtcNow.Date.AddDays(-30);
            var end = to ?? DateTime.UtcNow;

            var orders = dataContext.Orders.Where(o => o.OrderDate >= start && o.OrderDate <= end).ToList();
            var totalOrders = orders.Count;

            var successfulOrders = orders.Where(o => o.Status == "Paid" || o.Status == "Completed" || o.Status == "Delivered").ToList();
            var paidOrders = successfulOrders.Count;
            
            var netRevenue = successfulOrders.Sum(o => o.TotalPrice);
            var grossRevenue = orders.Sum(o => o.TotalPrice);

            return new SalesReportMV
            {
                From = start,
                To = end,
                TotalOrders = totalOrders,
                PaidOrders = paidOrders,
                GrossRevenue = grossRevenue,
                NetRevenue = netRevenue,
                TotalDiscounts = grossRevenue - netRevenue,
                TotalRevenue = netRevenue,
                TotalOrdersCount = paidOrders
            };
        }

        private Order GetOrder(int orderId, string orderType)
        {
            return orderType.ToLower() switch
            {
                "delivery" => dataContext.DeliveryOrders.FirstOrDefault(o => o.Id == orderId)
                    ?? throw new Exception("Delivery order not found."),
                "dinein" => dataContext.DineinOrders.FirstOrDefault(o => o.Id == orderId)
                    ?? throw new Exception("Dine-in order not found."),
                "takeaway" => dataContext.TakeawayOrders.FirstOrDefault(o => o.Id == orderId)
                    ?? throw new Exception("Takeaway order not found."),
                _ => throw new Exception("Unsupported order type.")
            };
        }

        private decimal GetDiscountAmount(string? offerCode, decimal grossAmount)
        {
            if (string.IsNullOrWhiteSpace(offerCode))
            {
                return 0;
            }

            var offer = dataContext.Offers.FirstOrDefault(o => o.Code == offerCode && o.IsActive);
            if (offer == null)
            {
                return 0;
            }

            var percentageDiscount = grossAmount * (offer.DiscountPercentage / 100m);
            var flatDiscount = offer.FlatDiscountAmount ?? 0m;
            return Math.Min(grossAmount, percentageDiscount + flatDiscount);
        }

        private void MarkOrderAsPaid(int orderId, string orderType, bool isPaid)
        {
            switch (orderType.ToLower())
            {
                case "delivery":
                    var delivery = dataContext.DeliveryOrders.FirstOrDefault(o => o.Id == orderId);
                    if (delivery != null)
                    {
                        delivery.IsPaid = isPaid;
                        if (isPaid && delivery.Status == "Pending")
                        {
                            delivery.Status = "Paid";
                        }
                    }
                    break;
                case "takeaway":
                    var takeaway = dataContext.TakeawayOrders.FirstOrDefault(o => o.Id == orderId);
                    if (takeaway != null)
                    {
                        takeaway.IsPaid = isPaid;
                        if (isPaid && takeaway.Status == "Pending")
                        {
                            takeaway.Status = "Paid";
                        }
                    }
                    break;
                case "dinein":
                    var dinein = dataContext.DineinOrders.FirstOrDefault(o => o.Id == orderId);
                    if (dinein != null && isPaid && dinein.Status == "Pending")
                    {
                        dinein.Status = "Paid";
                    }
                    break;
            }
        }

        private static PaymentMV Map(PaymentRecord payment, decimal discountAmount)
        {
            return new PaymentMV
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                OrderType = payment.OrderType,
                Amount = payment.Amount,
                DiscountAmount = discountAmount,
                NetAmount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                ReceiptNumber = payment.ReceiptNumber,
                PaymentUrl = payment.PaymentUrl,
                CreatedAt = payment.CreatedAt
            };
        }
    }
}
