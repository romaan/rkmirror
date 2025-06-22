using PsychologistBooking.Application.Dtos;
using PsychologistBooking.Application.Filters;
using PsychologistBooking.Domain.Enums;
using PsychologistBooking.Domain.Interfaces;

namespace PsychologistBooking.Application.UseCases;

public class GetPsychologistsUseCase : IGetPsychologistsUseCase
{
    private readonly IPsychologistRepository _repository;

    public GetPsychologistsUseCase(IPsychologistRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedResultDto<PsychologistDto>> ExecuteAsync(PsychologistFilterDto filter)
    {
        var random = new Random();
        
        var (psychologists, totalCount) = await _repository.GetPaginatedAsync(
            name: filter.Name,
            type: filter.Type,
            page: filter.Page,
            pageSize: filter.PageSize
        );

        var items = psychologists.Select(p => new PsychologistDto
        {
            Id = p.Id,
            Name = $"{p.FirstName} {p.LastName}",
            PsychologistType = p.PsychologistType.ToString(),
            PictureUrl = $"/images/0{random.Next(1, 6)}.jpg",
            NextAvailable = p.AvailableDates
                .OrderBy(d => d.Date)
                .Select(d => d.Date)
                .ToList()
        }).ToList();

        return new PaginatedResultDto<PsychologistDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }
}