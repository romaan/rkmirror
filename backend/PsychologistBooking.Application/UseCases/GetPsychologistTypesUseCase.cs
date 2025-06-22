using PsychologistBooking.Domain.Enums;

namespace PsychologistBooking.Application.UseCases;

public class GetPsychologistTypesUseCase : IGetPsychologistTypesUseCase
{
    public IEnumerable<string> Execute()
    {
        return Enum.GetNames(typeof(PsychologistType));
    }
}