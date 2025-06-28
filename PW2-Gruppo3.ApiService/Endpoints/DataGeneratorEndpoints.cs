using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PW2_Gruppo3.ApiService.Converters;
using PW2_Gruppo3.ApiService.Services;
using PW2_Gruppo3.DataGenerator;
using PW2_Gruppo3.Models;
using System.Text.Json;

namespace PW2_Gruppo3.ApiService;

public static class DataGeneratorEndpoints
{
    public static IEndpointRouteBuilder MapDataGeneratorEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/v1/telemetry")// Raggruppo tutte le api ineerenti allo stesso gruppo, non serve riscrivere tutto l'url
            .WithTags("DataGenerator")
            .WithOpenApi();
        
        group.MapPost("/machines", ReceiveTelemetryAsync)
            .WithName("SendTelemetry")
            .WithSummary("Send Telemetry Data");
        
        return builder;
    }

    private static async Task<Results<Ok<ReceivedData>, NoContent>> ReceiveTelemetryAsync(ReceivedData data, BatchAssociationService batchAssociationService)
    {
        // TODO: fare il CAST fra il messaggio ricevuto (Models.ReceivedMessage) e il messaggio inviato (DataGenerator.Models)
        await batchAssociationService.ProcessTelemetryMessage(data); 

        // Creo il percorso per il file di log
        //string logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        //Directory.CreateDirectory(logPath);
        
        //string logFile = Path.Combine(logPath, $"telemetry_log_{DateTime.Now:yyyy-MM-dd}.txt");
        //string jsonMessage = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        //string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Messaggio ricevuto:\n{jsonMessage}\n\n";
        
        //await File.AppendAllTextAsync(logFile, logEntry);

        return TypedResults.Ok(data);
    }

    // TODO: fare il CAST fra il messaggio ricevuto (Models.ReceivedData) e il messaggio inviato (DataGenerator.Message)
    private static async Task<ReceivedData> CastReceivedMessage(Message message, ReceivedData receivedData)
    {
        //  e qui dentro in qualche modo usiamo i converters creati
        return receivedData;
    }




}
