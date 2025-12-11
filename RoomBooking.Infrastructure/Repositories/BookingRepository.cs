using Microsoft.EntityFrameworkCore;

using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Models;
using RoomBooking.Infrastructure.Data;

namespace RoomBooking.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings.ToListAsync();
        }

        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
        }

        public async Task<List<Booking>> GetBookingsByRoomIdAsync(int roomId)
        {
            return await _context.Bookings
                .Where(b => b.RoomId == roomId)
                .ToListAsync();
        }

        public async Task<bool> AddBookingAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            
            var existing = await _context.Bookings.FindAsync(booking.Id);
            if (existing == null)
                return false;
            
            existing.RoomId = booking.RoomId;
            existing.StartDate = booking.StartDate;
            existing.EndDate = booking.EndDate;
            existing.UserId = booking.UserId;
            
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> DeleteBookingByIdAsync(int bookingId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null)
                return false;

            _context.Bookings.Remove(booking);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AlreadyExistsAsync(int bookingId)
        {
            return await _context.Bookings.AnyAsync(b => b.Id == bookingId);
        }
    }
}