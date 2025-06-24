using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomBookingC.Data;
using RoomBookingC.Models;
using RoomBookingC.Dtos;

namespace RoomBookingC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly RoomBookContext _context;

    public BookingController(RoomBookContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IEnumerable<Booking>> GetAll() =>
        await _context.Bookings.ToListAsync();

    [HttpGet("date/{date}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByDate(string date)
    {
        if (!DateTime.TryParse(date, out var targetDate))
            return BadRequest("Ungültiges Datum.");

        var start = targetDate.Date;
        var end = start.AddDays(1);

        var username = "Entwickler"; // ggf. dynamisch später
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Name == username);

        var bookings = await _context.Bookings
            .Where(b => b.StartTime >= start && b.StartTime < end)
            .Select(b => new BookingDto
            {
                Id = b.Id,
                RoomId = b.RoomId,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                CanDelete = employee != null && b.EmployeeId == employee.Id
            })
            .ToListAsync();

        return bookings;
    }


    [HttpPost]
    public async Task<IActionResult> Create(Booking booking)
    {
        // Für Entwicklung: Verwende einen statischen Benutzernamen
        var username = "Entwickler"; // In Produktion: User.Identity?.Name

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Name == username);
        if (employee == null)
        {
            employee = new Employee { Name = username };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        booking.EmployeeId = employee.Id;

        if (booking.StartTime < DateTime.Now)
            return BadRequest("Startzeit liegt in der Vergangenheit.");

        bool conflict = await _context.Bookings.AnyAsync(b =>
            b.RoomId == booking.RoomId &&
            booking.StartTime < b.EndTime &&
            booking.EndTime > b.StartTime);

        if (conflict)
            return BadRequest("Raum ist belegt.");

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return Ok(booking);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        // Für Entwicklung: Verwende denselben Dummy-Nutzer
        var username = "Entwickler";

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Name == username);
        if (employee == null)
            return NotFound("Mitarbeiter nicht gefunden.");

        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
            return NotFound();

        if (booking.EmployeeId != employee.Id)
            return Forbid("Du kannst nur deine eigenen Buchungen löschen.");

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
