using Microsoft.EntityFrameworkCore;
using PsychologistBooking.Domain.Entities;

namespace PsychologistBooking.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Psychologist> Psychologists => Set<Psychologist>();
    public DbSet<AvailableDate> AvailableDates => Set<AvailableDate>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}