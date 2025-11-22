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
        if(result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Booking>> AddBooking([FromBody] Booking booking)
    {
        if(booking == null || string.IsNullOrWhiteSpace(booking.EmployeeId))
            return BadRequest("Employee ID ist erforderlich");
        
        var (added, error) = await 
            _bookingService.AddBookingAsync(booking.EmployeeId, booking);
        if(!added)
            return BadRequest(error);

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Booking>> UpdateBooking(int id, [FromBody] Booking booking)
    {
        if (booking.EndDate <= booking.StartDate)
        {
            return BadRequest("EndTime ist vor StartTime");
        }

        if (booking.StartDate < DateTime.Now)
        {
            return BadRequest("StartTime ist in der Vergangenheit");
        }
        
        if(booking == null || id != booking.Id)
            return BadRequest("Booking ID ist null oder Id stimmt nicht überein");

        var (updated, error) = await _bookingService.UpdateBookingAsync(booking);
        if(!updated)
            return BadRequest(error);
        
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Booking>> DeleteBooking(int id)
    {
        if (id <= 0)
            return BadRequest(new { error = "Ungültige Id" });
        
        var booking = await _bookingService.GetBookingByIdAsync(id);
        if(booking == null)
            return NotFound();
        
        var (deleted, error) = await _bookingService.DeleteBookingAsync(booking);
        if(!deleted)
            return BadRequest(error);
        
        return Ok();
    }
}