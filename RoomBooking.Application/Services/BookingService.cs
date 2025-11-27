using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Models;


namespace RoomBooking.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<BookingService> _logger;

    public BookingService(IBookingRepository bookingRepository, ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<List<Booking>> GetAllBookingsAsync()
    {
        return await _bookingRepository.GetAllBookingsAsync();
    }

    public async Task<Booking?> GetBookingByIdAsync(int bookingId)
    {
        if (bookingId <= 0)
        {
            _logger.LogWarning("Booking Id must be greater than zero.");
            throw new ArgumentException("Booking Id must be greater than zero.", nameof(bookingId));
        }

        return await _bookingRepository.GetBookingByIdAsync(bookingId);
    }

    public async Task<(bool added, string? error)> AddBookingAsync(int userId, Booking booking)
    {
        if (userId <= 0)
        {
            _logger.LogWarning("AddBookingAsync: unbekannter Benutzer");
            return (false, "Unbekannter Benutzer");
        }

        if (booking.StartDate < DateTime.UtcNow)
        {
            _logger.LogWarning("AddBookingAsync: StartTime darf nicht in der Vergangenheit liegen");
            return (false,  "StartTime in der Vergangenheit");
        }

        if (booking.EndDate <= booking.StartDate)
        {
            _logger.LogWarning("AddBookingAsync: EndTime darf nicht vor oder gleich StartTime sein");
            return (false,  "EndTime früher oder gleich StartTime");
        }
        
    
        var bookingsInRoom = await _bookingRepository.GetBookingsByRoomIdAsync(booking.RoomId);
        bool overlaps = bookingsInRoom.Any(b =>
            b.StartDate < booking.EndDate && b.EndDate > booking.StartDate);

        if (overlaps)
        {
            _logger.LogWarning("AddBookingAsync: Buchung besteht bereits");
            return (false, "Buchung besteht bereits");
        }
        
        var ok = await _bookingRepository.AddBookingAsync(booking);
        if (!ok)
        {
            _logger.LogError("AddBookingAsync: Fehler beim Speichern");
            return (false, "Fehler beim Speichern");
        }
        return (true, null);
    }

    public async Task<(bool updated, string? error)> UpdateBookingAsync(Booking booking)
    {
        if (booking == null)
        {
            _logger.LogWarning("UpdateBookingAsync: Booking ist null");
            return (false, "Booking darf nicht null sein.");
        }

        if (booking.Id <= 0)
        {
            _logger.LogWarning("UpdateBookingAsync: Ungültige Booking-Id: {Id}", booking.Id);
            return (false, "Ungültige Booking-Id.");
        }

        var existing = await _bookingRepository.GetBookingByIdAsync(booking.Id);
        if (existing == null)
        {
            _logger.LogWarning("UpdateBookingAsync: Booking mit Id {Id} nicht gefunden", booking.Id);
            return (false, $"Booking mit Id {booking.Id} nicht gefunden.");
        }

        if (booking.StartDate >= booking.EndDate)
        {
            _logger.LogWarning("UpdateBookingAsync: Startzeit muss vor Endzeit liegen.");
            return (false, "Startzeit muss vor Endzeit liegen.");
        }

        if (booking.StartDate < DateTime.Now)
        {
            _logger.LogWarning("UpdaateBookingAsync: StartTime ist in der Vergangenheit");
            return (false, "StartTime in der Vergangenheit");
        }
        
        var bookingsInRoom = await _bookingRepository.GetBookingsByRoomIdAsync(booking.RoomId);
        bool overlaps = bookingsInRoom.Any(b =>
            b.Id != booking.Id &&
            b.StartDate < booking.EndDate && b.EndDate > booking.StartDate);

        if (overlaps)
        {
            _logger.LogWarning("UpdateBookingAsync: Überschneidung mit bestehender Buchung");
            return (false, "Raum ist im gewünschten Zeitraum bereits gebucht.");
        }

        var result = await _bookingRepository.UpdateBookingAsync(booking);
        if (!result)
        {
            _logger.LogError("UpdateBookingAsync: Fehler beim Aktualisieren der Buchung mit Id {Id}", booking.Id);
            return (false, "Fehler beim Aktualisieren der Buchung.");
        }

        return (true, null);
    }

    public async Task<(bool deleted, string? error)> DeleteBookingAsync(Booking booking)
    {
        if (booking == null)
        {
            _logger.LogWarning("DeleteBookingAsync: Booking ist null");
            return (false, "Booking darf nicht null sein.");
        }

        if (booking.Id <= 0)
        {
            _logger.LogWarning("DeleteBookingAsync: Ungültige BookingId");
            return(false,"Ungültige Booking-Id.");
        }

        if (booking.UserId <= 0)
        {
            _logger.LogWarning("DeleteBookingAsync: Leere Employee ID");
            return(false,"Leere User ID");
        }
        
        _logger.LogInformation("Lösche Booking mit Id {BookingId} für User {UserId}", booking.Id, booking.UserId);

        var deleted = await _bookingRepository.DeleteBookingByIdAsync(booking.Id);
        if (!deleted)
        {
            _logger.LogError("DeleteBookingAsync: Fehler beim Löschen der Buchung mit Id {BookingId}", booking.Id);
            return (false, "Fehler beim Löschen der Buchung.");
        }

        return (true, null);
        
    }

}