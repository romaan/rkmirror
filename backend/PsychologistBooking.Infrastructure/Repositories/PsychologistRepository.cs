using Microsoft.EntityFrameworkCore;
using PsychologistBooking.Domain.Entities;
using PsychologistBooking.Domain.Enums;
using PsychologistBooking.Domain.Interfaces;
using PsychologistBooking.Infrastructure.Data;


namespace PsychologistBooking.Infrastructure.Persistence.Repositories;

public class PsychologistRepository : IPsychologistRepository
{
    private readonly AppDbContext _dbContext;

    public PsychologistRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(List<Psychologist> Items, int TotalCount)> GetPaginatedAsync(
        string? name,
        PsychologistType? type,
        int page,
        int pageSize)
    {
        var query = _dbContext.Psychologists.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p =>
                p.FirstName.ToLower().Contains(name) ||
                p.LastName.ToLower().Contains(name));
        }

        if (type.HasValue)
        {
            query = query.Where(p => p.PsychologistType == type);
        }

        // Get total count before pagination
        var total = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Include(p => p.AvailableDates)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}