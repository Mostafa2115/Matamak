using Core.DTO;
using Core.IReprosatory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Resturant.Controllers
{
    [Route("api/[controller]")]
    [Route("api/v1/offers")]
    [ApiController]
    public class OffersController : ControllerBase
    {
        private readonly IOfferRepo offerRepo;

        public OffersController(IOfferRepo offerRepo)
        {
            this.offerRepo = offerRepo;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] bool onlyActive = false)
        {
            return Ok(offerRepo.GetAll(onlyActive));
        }

        [HttpGet("{code}")]
        public IActionResult GetByCode(string code)
        {
            var offer = offerRepo.GetByCode(code);
            return offer == null ? NotFound() : Ok(offer);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Add([FromBody] OfferD offer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(offerRepo.Add(offer));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] OfferD offer)
        {
            return Ok(offerRepo.Update(id, offer));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            offerRepo.Delete(id);
            return Ok("Offer removed successfully.");
        }
    }
}
