namespace PsychologistBooking.Application.UseCases;

public interface IGetPsychologistTypesUseCase
{
    IEnumerable<string> Execute();
}
