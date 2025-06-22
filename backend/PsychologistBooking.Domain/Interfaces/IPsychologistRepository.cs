using PsychologistBooking.Domain.Entities;
using PsychologistBooking.Domain.Enums;

namespace PsychologistBooking.Domain.Interfaces;

public interface IPsychologistRepository
{
    Task<(List<Psychologist> Items, int TotalCount)> GetPaginatedAsync(
        string? name,
        PsychologistType? type,
        int page,
        int pageSize);
}