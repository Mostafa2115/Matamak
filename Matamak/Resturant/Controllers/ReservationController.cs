using Core.DTO;
using Core.IReprosatory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Resturant.Controllers
{
    [Route("api/[controller]")]
    [Route("api/v1/reservations")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationRepo reservationRepo;

        public ReservationController(IReservationRepo reservationRepo)
        {
            this.reservationRepo = reservationRepo;
        }

        [Authorize(Roles = "Admin,Cashier")]
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(reservationRepo.GetAll());
        }

        [Authorize]
        [HttpPost]
        public IActionResult Add([FromBody] ReservationD reservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(reservationRepo.Add(reservation));
        }

        [Authorize(Roles = "Admin,Cashier")]
        [HttpPut("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromQuery] string status)
        {
            return Ok(reservationRepo.UpdateStatus(id, status));
        }
    }
}
