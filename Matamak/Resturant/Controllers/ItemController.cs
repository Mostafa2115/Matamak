using Core.DTO;
using Core.IReprosatory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Resturant.Controllers
{
    [Route("api/[controller]")]
    [Route("api/v1/items")]
    [Route("api/items")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemRepo itemRepo;

        public ItemController(IItemRepo itemRepo)
        {
            this.itemRepo = itemRepo;
        }

        [HttpGet]
        [Route("getAllItem")]
        [Route("")]
        public IActionResult GetAllItem()
        {
            var items = itemRepo.GetAllItems();
            if (items == null || items.Count == 0)
            {
                return NotFound("No items found.");
            }

            return Ok(items);
        }

        [HttpGet]
        [Route("getItemById/{id}")]
        [Route("{id}")]
        public IActionResult GetItem([FromRoute] int id)
        {
            var item = itemRepo.GetItemById(id);
            if (item == null)
            {
                return NotFound("Item not found.");
            }

            return Ok(item);
        }

        [HttpGet("sortItems")]
        public IActionResult GetItemsByCountryAndCategory([FromQuery] int? countryId, [FromQuery] int? categoryId)
        {
            var items = itemRepo.GetItensByCountryAndCategory(countryId, categoryId);
            if (items == null || items.Count == 0)
            {
                return NotFound("No items found for the specified country and category.");
            }

            return Ok(items);
        }

        [HttpGet("search")]
        public IActionResult Search([FromQuery] string term)
        {
            var items = itemRepo.GetAllItems()
                .Where(i =>
                    (!string.IsNullOrWhiteSpace(i.Name) && i.Name.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(i.Description) && i.Description.Contains(term, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return Ok(items);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("addItem")]
        public IActionResult AddItem([FromBody] ItemD item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                itemRepo.AddItem(item);
                return Ok("Item added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("updateItem/{id}")]
        public IActionResult UpdateItem([FromBody] ItemD item, [FromRoute] int id)
        {
            var existingItem = itemRepo.GetItemById(id);
            if (existingItem == null)
            {
                return NotFound("Item not found.");
            }

            itemRepo.UpdateItem(item, id);
            return Ok("Item updated successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("removeItem/{id}")]
        [Route("removeItem")]
        public IActionResult RemoveItem([FromRoute] int id)
        {
            var existingItem = itemRepo.GetItemById(id);
            if (existingItem == null)
            {
                return NotFound("Item not found.");
            }

            itemRepo.RemoveItem(id);
            return Ok("Item removed successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("uploadImage")]
        public async Task<IActionResult> UploadImage([FromForm] ImageUploadD model, [FromServices] IWebHostEnvironment env)
        {
            if (model == null || model.File == null || model.File.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var file = model.File;
            var webRootPath = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
            var uploadsFolderPath = Path.Combine(webRootPath, "uploads");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var fileUrl = $"/uploads/{uniqueFileName}";
            return Ok(new { url = fileUrl });
        }
    }

    public class ImageUploadD
    {
        public IFormFile File { get; set; } = null!;
    }
}
