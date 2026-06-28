using Core.DTO;
using Core.IReprosatory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Resturant.Controllers
{
    [Route("api/[controller]")]
    [Route("api/v1/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepo reviewRepo;

        public ReviewController(IReviewRepo reviewRepo)
        {
            this.reviewRepo = reviewRepo;
        }

        [HttpGet("item/{itemId}")]
        public IActionResult GetByItem(int itemId)
        {
            return Ok(reviewRepo.GetByItem(itemId));
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public IActionResult Add([FromBody] ReviewD review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name ?? "customer";
            return Ok(reviewRepo.Add(username, review));
        }
    }
}
