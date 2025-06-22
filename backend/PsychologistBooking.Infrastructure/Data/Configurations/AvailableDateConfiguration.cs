using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychologistBooking.Domain.Entities;

public class AvailableDateConfiguration : IEntityTypeConfiguration<AvailableDate>
{
    public void Configure(EntityTypeBuilder<AvailableDate> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Date)
            .IsRequired();

        builder.Property(a => a.PsychologistId)
            .IsRequired();

        builder.HasOne<Psychologist>()
            .WithMany(p => p.AvailableDates)
            .HasForeignKey(a => a.PsychologistId);
    }
}