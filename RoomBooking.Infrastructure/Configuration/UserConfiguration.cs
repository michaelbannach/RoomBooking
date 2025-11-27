using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RoomBooking.Domain.Models;
using RoomBooking.Infrastructure.Models;

namespace RoomBooking.Infrastructure.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd()  
            .IsRequired();
        
        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(50);
        
       builder.HasOne<ApplicationUser>()
           .WithOne()
           .HasForeignKey<User>(e => e.IdentityUserId)
           .OnDelete(DeleteBehavior.Cascade);
        
        
    }
}