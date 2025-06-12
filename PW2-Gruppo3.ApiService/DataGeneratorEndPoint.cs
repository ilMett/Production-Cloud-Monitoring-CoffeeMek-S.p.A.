using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using PW2_Gruppo3.DataGenerator;

namespace PW2_Gruppo3.ApiService;

public static class DataGeneratorEndPoint
{
    public static IEndpointRouteBuilder MapDataGeneratorEndPoint(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/v1/telemetry")// Raggruppo tutte le api inierenti allo stesso gruppo, non serve riscrivere tutto l'url
            .WithTags("DataGenerator")
            .WithOpenApi();
        
        group.MapPost("/machines", SendTelemetryAsync)
            .WithName("SendTelemetry")
            .WithSummary("Send Telemetry Data");
        
        return builder;
    }

    private static async Task<Results<Ok<Message>, NoContent>> SendTelemetryAsync(Message message)
    {
        return TypedResults.Ok(message);
    }




}
