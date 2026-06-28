using Core.DTO;
using Core.IReprosatory;
using Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Resturant.Controllers
{
    [Route("api/[controller]")]
    [Route("api/v1/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICatrgoryRepo catrgoryRepo;
        private readonly DataContext dataContext;

        public CategoryController(ICatrgoryRepo catrgoryRepo, DataContext dataContext)
        {
            this.catrgoryRepo = catrgoryRepo;
            this.dataContext = dataContext;
        }

        [HttpGet]
        [Route("getCategoryById")]
        [Route("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            try
            {
                var category = catrgoryRepo.GetCategory(id);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpGet]
        [Route("getAllCategories")]
        [Route("")]
        public IActionResult GetAllCategories()
        {
            var categories = catrgoryRepo.GetAllCategories();
            return Ok(categories);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("addCategory")]
        public IActionResult AddCategory(CategoryD category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            catrgoryRepo.AddCategory(category);
            return Ok("Category added successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("removeCategory")]
         public IActionResult DeleteCategory(int id)
         {
            try
            {
                catrgoryRepo.RemoveCategory(id);
                return Ok("Category removed successfully");
            }
            catch (Exception ex)
            {
                 return NotFound(ex.Message);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("editCategory")]
        public IActionResult EditCategory(CategoryD category, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            catrgoryRepo.UpdateCategory(category, id);
            return Ok("Category updated successfully");
        }

    }
}
