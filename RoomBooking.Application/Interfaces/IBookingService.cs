using RoomBooking.Domain.Models;

namespace RoomBooking.Application.Interfaces;

public interface IBookingService
{
    Task<List<Booking>> GetAllBookingsAsync();
    
    Task<Booking?> GetBookingByIdAsync(int bookingid);
    
    Task<(bool added, string? error)> AddBookingAsync(string employeeId, Booking booking);
    
    Task<(bool updated, string? error)> UpdateBookingAsync(Booking booking);
    
    Task<(bool deleted, string? error)> DeleteBookingAsync(Booking booking);
}