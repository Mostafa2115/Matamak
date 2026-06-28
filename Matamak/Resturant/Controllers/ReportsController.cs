using Core.IReprosatory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Resturant.Controllers
{
    [Route("api/[controller]")]
    [Route("api/v1/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IPaymentRepo paymentRepo;

        public ReportsController(IPaymentRepo paymentRepo)
        {
            this.paymentRepo = paymentRepo;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("sales")]
        public IActionResult Sales([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            return Ok(paymentRepo.GetSalesReport(from, to));
        }
    }
}
