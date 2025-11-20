using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RoomBooking.Domain.Models;

namespace RoomBooking.Infrastructure.Configuration;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(e => e.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);
    }
}