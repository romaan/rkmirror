using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Http;
using PsychologistBooking.Application.UseCases;

namespace PsychologistBooking.Functions;

public class GetPsychologistTypes
{
    private readonly IGetPsychologistTypesUseCase _useCase;

    public GetPsychologistTypes(IGetPsychologistTypesUseCase useCase)
    {
        _useCase = useCase;
    }

    [Function("GetPsychologistTypes")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "psychologist-types")] HttpRequestData req)
    {
        var types = _useCase.Execute();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(types);
        return response;
    }
}
