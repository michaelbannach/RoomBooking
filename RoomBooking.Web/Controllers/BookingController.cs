using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Models;


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

    [HttpGet]
    public async Task<ActionResult<List<Booking>>> GetAllBookings()
    {
        var result = await _bookingService.GetAllBookingsAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Booking>> GetBookingById(int id)
    {
        var result = await _bookingService.GetBookingByIdAsync(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddBooking([FromBody] Booking booking)
    {
        if (booking == null)
            return BadRequest("Booking darf nicht null sein.");

        // Businessregeln (Zeiten, Overlaps, usw.) sind im BookingService
        var (added, error) = await _bookingService.AddBookingAsync(booking.UserId, booking);
        if (!added)
            return BadRequest(error);

        return Ok(booking);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBooking(int id, [FromBody] Booking booking)
    {
        if (booking == null || id != booking.Id)
            return BadRequest("Booking ID ist null oder Id stimmt nicht überein");

        var (updated, error) = await _bookingService.UpdateBookingAsync(booking);
        if (!updated)
            return BadRequest(error);

        return Ok(booking);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        if (id <= 0)
            return BadRequest(new { error = "Ungültige Id" });

        var booking = await _bookingService.GetBookingByIdAsync(id);
        if (booking == null)
            return NotFound();

        var (deleted, error) = await _bookingService.DeleteBookingAsync(booking);
        if (!deleted)
            return BadRequest(error);

        return Ok();
    }

}