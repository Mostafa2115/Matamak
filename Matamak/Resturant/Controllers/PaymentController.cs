using Core.DTO;
using Core.IReprosatory;
using Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Resturant.Controllers
{
    [Route("api/[controller]")]
    [Route("api/v1/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymobService paymobService;
        private readonly IPaymentRepo paymentRepo;

        public PaymentController(IPaymobService paymobService, IPaymentRepo paymentRepo)
        {
            this.paymobService = paymobService;
            this.paymentRepo = paymentRepo;
        }

        [Authorize(Roles = "Cashier,Customer")]
        [HttpPost]
        public async Task<IActionResult> Process([FromBody] PaymentRequestD request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(await paymentRepo.ProcessPaymentAsync(request));
        }

        [Authorize(Roles = "Cashier,Admin")]
        [HttpPut("confirm")]
        public IActionResult Confirm([FromBody] PaymentConfirmD request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(paymentRepo.ConfirmPayment(request));
        }

        [Authorize(Roles = "Cashier,Admin")]
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(paymentRepo.GetAll());
        }

        [HttpPost("pay/{orderId}")]
        public async Task<IActionResult> Pay(int orderId)
        {
            var paymentUrl = await paymobService.GetPaymentUrlAsync(orderId, 15000, "customer@matamak.local");
            return Ok(new { paymentUrl });
        }
    }
}
