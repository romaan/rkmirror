using PsychologistBooking.Application.Dtos;
using PsychologistBooking.Application.Filters;

namespace PsychologistBooking.Application.UseCases;

public interface IGetPsychologistsUseCase
{
    Task<PaginatedResultDto<PsychologistDto>> ExecuteAsync(PsychologistFilterDto filter);
}