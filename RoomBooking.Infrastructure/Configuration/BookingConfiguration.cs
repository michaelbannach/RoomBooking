using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RoomBooking.Domain.Models;

namespace RoomBooking.Infrastructure.Configuration;

public class BookingConfiguration  : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);
        
        
        
        builder.Property(b => b.StartDate)
            .IsRequired();
        
        builder.Property(b => b.EndDate)
            .IsRequired();
        
        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(b => b.Room)
            .WithMany()
            .HasForeignKey(b => b.RoomId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}