using Core.DTO;
using Core.IReprosatory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Resturant.Controllers
{
    [Route("api/[controller]")]
    [Route("api/v1/inventory")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepo inventoryRepo;

        public InventoryController(IInventoryRepo inventoryRepo)
        {
            this.inventoryRepo = inventoryRepo;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(inventoryRepo.GetAll());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("low-stock")]
        public IActionResult GetLowStock()
        {
            return Ok(inventoryRepo.GetLowStockItems());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Upsert([FromBody] InventoryItemD inventoryItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(inventoryRepo.AddOrUpdate(inventoryItem));
        }
    }
}
