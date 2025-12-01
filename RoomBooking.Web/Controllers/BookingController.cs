using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.Interfaces;
using RoomBooking.Web.Dtos.Bookings;
using RoomBooking.Web.Mappings;

namespace RoomBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // GET: api/booking
    [HttpGet]
    public async Task<ActionResult<List<BookingDto>>> GetAllBookings()
    {
        var bookings = await _bookingService.GetAllBookingsAsync();
        return Ok(bookings.ToDto().ToList());
    }

    // GET: api/booking/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDto>> GetBookingById(int id)
    {
        if (id <= 0)
            return BadRequest("Invalid BookingId");

        var booking = await _bookingService.GetBookingByIdAsync(id);
        if (booking == null)
            return NotFound();

        return Ok(booking.ToDto());
    }

    // POST: api/booking
    [HttpPost]
    public async Task<ActionResult<BookingDto>> AddBooking([FromBody] BookingCreateDto dto)
    {
        if (dto == null)
            return BadRequest("Booking is null.");

        // Domain-Entity aus DTO erzeugen
        var booking = dto.ToEntity();

        // Alle fachlichen Regeln (Zeiten, Overlaps usw.) sind im Service
        var (added, error) = await _bookingService.AddBookingAsync(dto.UserId, booking);
        if (!added)
            return BadRequest(error);

        return Ok(booking.ToDto());
    }

    // PUT: api/booking/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BookingDto>> UpdateBooking(int id, [FromBody] BookingUpdateDto dto)
    {
        if (dto == null || id != dto.Id)
            return BadRequest("BookingId is null or mismatch");

        if (id <= 0)
            return BadRequest("Invalid Id");

        var existing = await _bookingService.GetBookingByIdAsync(id);
        if (existing == null)
            return NotFound();

        // Apply dtos on existing entities
        dto.Apply(existing);

        var (updated, error) = await _bookingService.UpdateBookingAsync(existing);
        if (!updated)
            return BadRequest(error);

        return Ok(existing.ToDto());
    }

    // DELETE: api/booking/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        if (id <= 0)
            return BadRequest(new { error = "Invalid Id" });

        var booking = await _bookingService.GetBookingByIdAsync(id);
        if (booking == null)
            return NotFound();

        var (deleted, error) = await _bookingService.DeleteBookingAsync(booking);
        if (!deleted)
            return BadRequest(error);

        return Ok();
    }
}
