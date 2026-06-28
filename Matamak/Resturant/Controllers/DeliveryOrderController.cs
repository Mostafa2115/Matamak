using Core.DTO;
using Core.IReprosatory;
using Core.Models;
using Core.ModelView;
using Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.EntityFrameworkCore;

namespace Resturant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryOrderController : ControllerBase
    {
        private readonly IOrderItemsRepo orderItemsRepo;
        private readonly DataContext dataContext;
        private readonly IDeliveryOrderRepo deliveryOrderRepo;

        public DeliveryOrderController(IOrderItemsRepo orderItemsRepo, DataContext dataContext, IDeliveryOrderRepo deliveryOrderRepo)
        {
            this.orderItemsRepo = orderItemsRepo;
            this.dataContext = dataContext;

            this.deliveryOrderRepo = deliveryOrderRepo;
        }




        [Authorize(Roles ="Admin,Cashier")]
        [HttpGet("getDeliveryOrders")]
        public IActionResult GetDeliveryOrders()
        {


            var orders = deliveryOrderRepo.GetAllDeliveryOrders();
            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found");
            }
            return Ok(orders);
        }



        [Authorize]
        [HttpGet("getDeliveryOrderById/{id}")]
        public IActionResult GetDeliveryOrder([FromRoute] int id)
        {
            var order = deliveryOrderRepo.GetDeliveryOrder(id);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            return Ok(order);
        }


        [Authorize(Roles ="Customer")]
        [HttpPost("addDelveryOrder")]
        public async Task<IActionResult> AddDeliveryOrder(DelivaryD delivaryD)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            await deliveryOrderRepo.AddDeliveryOrder(delivaryD);
            return Ok("Add Succsefuly");
        }

        [Authorize(Roles = "Customer")]
        [HttpPut("updateDeliveryOrder/{id}")]
        public IActionResult UpdateDeliveryOrder([FromRoute] int id, [FromBody] DelivaryD delivaryD)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            try 
            {
                deliveryOrderRepo.UpdateDeliveryOrder(delivaryD, id);
                return Ok("Updated Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Customer,Cashier")]
        [HttpPut("cancelDeliveryOrder/{id}")]
        public async Task<IActionResult> CancelDeliveryOrder([FromRoute] int id)
        {
            try
            {
                await deliveryOrderRepo.CancelDeliveryOrder(id);
                return Ok("Canceled Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [Authorize(Roles ="Cashier")]
         [HttpPut("handOrderToDriver/{id}")]
         public async Task<IActionResult> HandOrderToDriver([FromRoute] int id)
         {
            try
            {
                await deliveryOrderRepo.HandOrderToDriver(id);
                return Ok("Handed to driver successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
         }

        [Authorize(Roles = "Customer")]
        [HttpPut("handOrderToCustomer/{id}")]
        public async Task<IActionResult> HandOrderToCustomer([FromRoute] int id)
        {
            try
            {
                await deliveryOrderRepo.HandOrderToCustmor(id);
                return Ok("Handed to customer successfully");
            
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("removeDeliveryOrder/{id}")]
        public IActionResult RemoveDeliveryOrder([FromRoute] int id)
        {
            try 
            {
                deliveryOrderRepo.RemoveDeliveryOrder(id);
                return Ok("Removed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}


