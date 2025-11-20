using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RoomBooking.Domain.Models;

namespace RoomBooking.Infrastructure.Configuration;

public class RoomConfiguration  : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired();
        
        builder.Property(r => r.Capacity)
            .IsRequired();
    }
}