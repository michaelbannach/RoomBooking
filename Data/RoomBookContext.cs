using Microsoft.EntityFrameworkCore;
using RoomBookingC.Models;

namespace RoomBookingC.Data;

public class RoomBookContext : DbContext
{
    public RoomBookContext(DbContextOptions<RoomBookContext> options) : base(options) {}
    
    public DbSet<Booking> Bookings => Set<Booking>();
    public  DbSet<Room> Rooms => Set<Room>();
    public DbSet<Employee> Employees => Set<Employee>();
}