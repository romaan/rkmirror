using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using PsychologistBooking.Application.UseCases;
using System.Web;
using PsychologistBooking.Application.Filters;
using PsychologistBooking.Domain.Enums;

namespace PsychologistBooking.Functions.Functions;

public class GetPsychologists
{
    private readonly IGetPsychologistsUseCase _getPsychologistsUseCase;

    public GetPsychologists(IGetPsychologistsUseCase getPsychologistsUseCase)
    {
        _getPsychologistsUseCase = getPsychologistsUseCase;
    }

    [Function("GetPsychologists")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "psychologists")] HttpRequestData req)
    {
        var query = HttpUtility.ParseQueryString(req.Url.Query);

        var filter = new PsychologistFilterDto
        {
            Name = query["name"],
            Type = ParsePsychologistType(query["type"]),
            Page = int.TryParse(query["page"], out var page) ? page : 1,
            PageSize = int.TryParse(query["pageSize"], out var size) ? size : 10
        };

        var result = await _getPsychologistsUseCase.ExecuteAsync(filter);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(result);
        return response;
    }
    
    private PsychologistType? ParsePsychologistType(string? typeStr)
    {
        if (!string.IsNullOrWhiteSpace(typeStr) &&
            Enum.TryParse<PsychologistType>(typeStr, ignoreCase: true, out var typeEnum))
        {
            return typeEnum;
        }

        return null;
    }

}