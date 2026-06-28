using Core.DTO;
using Core.IReprosatory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Resturant.Controllers
{
    [Route("api/[controller]")]
    [Route("api/v1/takeaway-orders")]
    [ApiController]
    public class TakeAwayOrderController : ControllerBase
    {
        private readonly ITakeAwayOrderRepo takeAwayOrderRepo;

        public TakeAwayOrderController(ITakeAwayOrderRepo takeAwayOrderRepo)
        {
            this.takeAwayOrderRepo = takeAwayOrderRepo;
        }

        [Authorize(Roles = "Admin,Cashier")]
        [HttpGet]
        [Route("")]
        [Route("getAllTakeAwayOrders")]
        public IActionResult GetAll()
        {
            return Ok(takeAwayOrderRepo.GetAllTakeAwayOrders());
        }

        [Authorize(Roles = "Admin,Cashier,Customer")]
        [HttpGet]
        [Route("{id}")]
        [Route("getTakeAwayOrder/{id}")]
        public IActionResult Get(int id)
        {
            return Ok(takeAwayOrderRepo.GetTakeAwayOrder(id));
        }

        [Authorize(Roles = "Customer,Cashier")]
        [HttpPost]
        [Route("")]
        [Route("addTakeAwayOrder")]
        public async Task<IActionResult> Add([FromBody] TakeAwayD order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await takeAwayOrderRepo.AddTakeAwayOrder(order);
            return Ok("Takeaway order created successfully.");
        }

        [Authorize(Roles = "Customer,Cashier")]
        [HttpPut]
        [Route("{id}")]
        [Route("updateTakeAwayOrder/{id}")]
        public IActionResult Update(int id, [FromBody] TakeAwayD order)
        {
            takeAwayOrderRepo.UpdateTakeAwayOrder(order, id);
            return Ok("Takeaway order updated successfully.");
        }

        [Authorize(Roles = "Admin,Cashier,Customer")]
        [HttpPut("changeTakeawayOrderStatus/{id}")]
        public async Task<IActionResult> ChangeTakeawayOrderStatus([FromRoute] int id, [FromQuery] string status)
        {
            if (User.IsInRole("Customer") && !status.Equals("Canceled", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Customers are only allowed to cancel their orders.");
            }

            try
            {
                await takeAwayOrderRepo.ChangeTakeawayOrderStatus(id, status);
                return Ok("Takeaway order status updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin,Cashier")]
        [HttpDelete]
        [Route("{id}")]
        [Route("removeTakeAwayOrder/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await takeAwayOrderRepo.RemoveTakeAwayOrder(id);
            return Ok("Takeaway order removed successfully.");
        }
    }
}

