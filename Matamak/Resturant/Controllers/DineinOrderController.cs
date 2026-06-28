using Core.DTO;
using Core.IReprosatory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Resturant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DineinOrderController : ControllerBase
    {
        private readonly IDineinOrderRepo dineinOrderRepo;

        public DineinOrderController(IDineinOrderRepo dineinOrderRepo)
        {
            this.dineinOrderRepo = dineinOrderRepo;
        }


        [Authorize(Roles ="Admin,Cashier")]
        [HttpGet("getAllDineinOrders")]
        public IActionResult GetAllDineinOrders()
        {
            
            var orders = dineinOrderRepo.GetAllDineinOrders();
            return Ok(orders);
        }

        [Authorize(Roles = "Admin,Cashier")]
        [HttpGet("getDineinOrder/{id}")]
        public IActionResult GetDineinOrder([FromRoute] int id)
        {
            try
            {
                var order = dineinOrderRepo.GetDineinOrder(id);

                return Ok(order);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "Cashier")]
        [HttpPost("addDineinOrder")]
        public IActionResult AddDineinOrder([FromBody] DineinD order)
        {
            try
            {
                dineinOrderRepo.AddDineinOrder(order);
                return Ok("Order added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin,Cashier")]
        [HttpPut("updateDineinOrder/{id}")]
        public IActionResult UpdateDineinOrder([FromBody] DineinD order, [FromRoute] int id)
        {
            try
            {
                dineinOrderRepo.UpdateDineinOrder(order, id);
                return Ok("Order updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin,Cashier")]
        [HttpPut("ChangeDineinOrderStatus/{id}")]
        public IActionResult ChangeDineinOrderStatus([FromRoute] int id, [FromQuery] string status)
        {
            try
            { 
                dineinOrderRepo.ChangeDineinOrderStatus(id, status);
                return Ok("Order status updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Cashier")]
        [HttpDelete("removeDineinOrder/{id}")]
        public IActionResult RemoveDineinOrder([FromRoute] int id)
        {
            try
            {
                dineinOrderRepo.RemoveDineinOrder(id);
                return Ok("Order removed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
