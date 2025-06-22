using PsychologistBooking.Domain.Enums;

namespace PsychologistBooking.Application.Filters;

public class PsychologistFilterDto
{
    public string? Name { get; set; }
    public PsychologistType? Type { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
