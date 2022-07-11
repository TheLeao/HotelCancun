using Application.Reservations;
using Application.Reservations.Services;
using Domain.Reservations;
using Microsoft.AspNetCore.Mvc;

namespace HotelCancun.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        /// <summary>
        /// Returns all current not cancelled reservations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Reservation>>> GetAllReservations()
        {
            return Ok(await _reservationService.GetReservationsAsync());
        }

        /// <summary>
        /// Returns a reservation based on the Id
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [HttpGet("id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReservationOperationResult>> GetById(long id)
        {
            Reservation? reservation = await _reservationService.GetReservationAsync(id);

            if (reservation != null)
                return Ok(reservation);

            return NotFound("No reservation found for this identifier");
        }

        /// <summary>
        /// Returns all canceled reservations
        /// </summary>
        /// <returns></returns>
        [HttpGet("canceled")]
        public async Task<ActionResult<List<Reservation>>> GetCancelled()
        {
            List<Reservation> reservations = await _reservationService.GetCancelledReservationsAsync();
            return Ok(reservations);
        }

        /// <summary>
        /// Create new Reservation
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReservationOperationResult>> PostReservation([FromBody] Reservation reservation)
        {
            ReservationOperationResult result = await _reservationService.CreateReservationAsync(reservation);

            if (result.Success)
                return Ok(result.Data);

            return UnprocessableEntity(result.Errors);
        }

        /// <summary>
        /// Modify a reservation
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReservationOperationResult>> PutReservation(long id, [FromBody] Reservation reservation)
        {
            if (id != reservation.Id)
                return BadRequest("The specified reservation Id does not match the route identifier.");

            ReservationOperationResult result = await _reservationService.ModifyReservationAsync(id, reservation);

            if (result.Success)
                return Ok(result.Data);

            return UnprocessableEntity(result.Errors);
        }

        /// <summary>
        /// Cancel a reservation
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [HttpPatch("cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReservationOperationResult>> CancelReservationById(long id)
        {
            ReservationOperationResult result = await _reservationService.CancelReservationAsync(id);

            if (result.Success)
                return Ok(result.Data);

            return NotFound("No reservations found for this identifier");
        }

        /// <summary>
        /// Returns all current reservations of specified guest
        /// </summary>
        /// <returns></returns>
        [HttpGet("reserved-by")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Reservation>>> GetReservationsByGuest(string reservedBy)
        {
            List<Reservation> reservations = await _reservationService.GetReservationByGuest(reservedBy);
            return Ok(reservations);
        }
    }
}
