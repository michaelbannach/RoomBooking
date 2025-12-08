using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.Interfaces;
using RoomBooking.Web.Dtos.Bookings;
using RoomBooking.Web.Mappings;

namespace RoomBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    
    [HttpGet]
    public async Task<ActionResult<List<BookingDto>>> GetAllBookings()
    {
        var bookings = await _bookingService.GetAllBookingsAsync();
        return Ok(bookings.ToDto().ToList());
    }

    
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

   
    [HttpPost]
    public async Task<ActionResult<BookingDto>> AddBooking([FromBody] BookingCreateDto dto)
    {
        if (dto == null)
            return BadRequest("Booking is null.");

        var userIdClaim = User.FindFirst("userId")?.Value;
        if(!int.TryParse(userIdClaim, out var userId) || userId <= 0)
            return Unauthorized(new {error ="UserId missing in token"});
        
        var booking = dto.ToEntity();

        booking.UserId = userId;
        
        var (added, error) = await _bookingService.AddBookingAsync(dto.UserId, booking);
        if (!added)
            return BadRequest(error);

        return Ok(booking.ToDto());
    }

    
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
