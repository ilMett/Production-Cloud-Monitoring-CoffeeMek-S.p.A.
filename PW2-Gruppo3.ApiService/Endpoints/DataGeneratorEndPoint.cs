using Microsoft.AspNetCore.Http.HttpResults;
using PW2_Gruppo3.ApiService.Services;
using PW2_Gruppo3.DataGenerator;
using System.Text.Json;

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
        //  integrazione nell'endpoint della chiamata al service BatchAssociationService
        var service = new BatchAssociationService();
        service.ProcessTelemetryMessage(message); // Elabora e istanzia oggetti

        // Creo il percorso per il file di log
        string logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        Directory.CreateDirectory(logPath);
        
        string logFile = Path.Combine(logPath, $"telemetry_log_{DateTime.Now:yyyy-MM-dd}.txt");
        string jsonMessage = JsonSerializer.Serialize(message, new JsonSerializerOptions { WriteIndented = true });
        string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Messaggio ricevuto:\n{jsonMessage}\n\n";
        
        await File.AppendAllTextAsync(logFile, logEntry);

        return TypedResults.Ok(message);
    }




}
