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
            _logger.LogWarning("BookingId must be greater than zero");
            throw new ArgumentException("BookingId must be greater than zero", nameof(bookingId));
        }

        return await _bookingRepository.GetBookingByIdAsync(bookingId);
    }

    public async Task<(bool added, string? error)> AddBookingAsync(int userId, Booking booking)
    {
        if (userId <= 0)
        {
            _logger.LogWarning("AddBookingAsync: Unknokn user");
            return (false, "Unknown user");
        }

        if (booking.StartDate < DateTime.UtcNow)
        {
            _logger.LogWarning("AddBookingAsync: StartTime in the past. Not allowed");
            return (false,  "StartTime in the past. Not Allowed");
        }

        if (booking.EndDate <= booking.StartDate)
        {
            _logger.LogWarning("AddBookingAsync: EndTime must not be earlier than or equal to StartTime");
            return (false,  "EndTime earlier or equal StartTime");
        }
        
    
        var bookingsInRoom = await _bookingRepository.GetBookingsByRoomIdAsync(booking.RoomId);
        bool overlaps = bookingsInRoom.Any(b =>
            b.StartDate < booking.EndDate && b.EndDate > booking.StartDate);

        if (overlaps)
        {
            _logger.LogWarning("AddBookingAsync: Booking already exists. Overlapping");
            return (false, "Booking already exists. Overlapping");
        }
        
        var ok = await _bookingRepository.AddBookingAsync(booking);
        if (!ok)
        {
            _logger.LogError("AddBookingAsync: Error while saving");
            return (false, "Error while saving");
        }
        return (true, null);
    }

    public async Task<(bool updated, string? error)> UpdateBookingAsync(Booking booking)
    {
        if (booking == null)
        {
            _logger.LogWarning("UpdateBookingAsync: Booking is null");
            return (false, "Booking is null. Not allowed");
        }

        if (booking.Id <= 0)
        {
            _logger.LogWarning("UpdateBookingAsync: Invalid Booking-Id: {Id}", booking.Id);
            return (false, "Invalid BookingId");
        }

        var existing = await _bookingRepository.GetBookingByIdAsync(booking.Id);
        if (existing == null)
        {
            _logger.LogWarning("UpdateBookingAsync: BookingId {Id} not found", booking.Id);
            return (false, $"BookingId {booking.Id} not found");
        }

        if (booking.StartDate >= booking.EndDate)
        {
            _logger.LogWarning("UpdateBookingAsync: StartTime must be earlier then EndTime");
            return (false, "StartTime must be earlier then EndTime");
        }

        if (booking.StartDate < DateTime.Now)
        {
            _logger.LogWarning("UpdaateBookingAsync: StartTime is in the past. Not allowed.");
            return (false, "StartTime in the past. Not Allowed");
        }
        
        var bookingsInRoom = await _bookingRepository.GetBookingsByRoomIdAsync(booking.RoomId);
        bool overlaps = bookingsInRoom.Any(b =>
            b.Id != booking.Id &&
            b.StartDate < booking.EndDate && b.EndDate > booking.StartDate);

        if (overlaps)
        {
            _logger.LogWarning("UpdateBookingAsync: Booking already exists. Overlapping");
            return (false, "Already booked. Overlapping");
        }

        var result = await _bookingRepository.UpdateBookingAsync(booking);
        if (!result)
        {
            _logger.LogError("UpdateBookingAsync: Failed to update Booking with Id {Id}", booking.Id);
            return (false, "Error while updating the booking");
        }

        return (true, null);
    }

    public async Task<(bool deleted, string? error)> DeleteBookingAsync(Booking booking)
    {
        if (booking == null)
        {
            _logger.LogWarning("DeleteBookingAsync: Booking is null");
            return (false, "Booking must not be null");
        }

        if (booking.Id <= 0)
        {
            _logger.LogWarning("DeleteBookingAsync: Invalid BookingId");
            return(false,"Invalid BookingId");
        }

        if (booking.UserId <= 0)
        {
            _logger.LogWarning("DeleteBookingAsync: Empty UserId");
            return(false,"Empty UserId");
        }
        
        _logger.LogInformation("Delete booking {BookingId} from user {UserId}", booking.Id, booking.UserId);

        var deleted = await _bookingRepository.DeleteBookingByIdAsync(booking.Id);
        if (!deleted)
        {
            _logger.LogError("DeleteBookingAsync: Error deleting BookingId {BookingId}", booking.Id);
            return (false, "Error deleting booking");
        }

        return (true, null);
        
    }

}