using RoomBooking.Domain.Models;

namespace RoomBooking.Application.Interfaces;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllBookingsAsync();

    Task<Booking> GetBookingByIdAsync(int bookingId);

    Task<List<Booking>> GetBookingsByRoomIdAsync(int roomId);

    Task<bool> AddBookingAsync(Booking booking);

    Task<bool> UpdateBookingAsync(Booking booking);

    Task<bool> DeleteBookingByIdAsync(int bookingId);

    Task<bool> AlreadyExistsAsync(int bookingId);

}