using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychologistBooking.Domain.Entities;

namespace PsychologistBooking.Infrastructure.Data.Configurations;

public class PsychologistConfiguration : IEntityTypeConfiguration<Psychologist>
{
    public void Configure(EntityTypeBuilder<Psychologist> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.ShortDescription)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.PsychologistType)
            .IsRequired();

        builder.HasMany(p => p.AvailableDates)
            .WithOne()
            .HasForeignKey(ad => ad.PsychologistId);
    }
}
