using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PW2_Gruppo3.ApiService.Data;
using PW2_Gruppo3.DataGenerator;
using PW2_Gruppo3.Models;


namespace PW2_Gruppo3.ApiService.Services;
//  batch: 3fa85f64-5717-4262-b3fc-2c963f66afa6
//  customer: 3fa85f64-5717-4562-b3fc-2c963f66afa6
//  site: 3fa85f64-5717-4562-b3fc-2c963f66afa6
public class BatchAssociationService
{
    private readonly ProductionMonitoringContext _context;
    private readonly IBatchQueueService _batchQueueService;
    private readonly IGenericService<Batch> _batchService;
    private readonly IGenericService<AssemblyLine> _assemblyLineService;
    private readonly IGenericService<Lathe> _latheService;
    private readonly IGenericService<Milling> _millingService;
    private readonly IGenericService<TestLine> _testLineService;

    public BatchAssociationService(
        IBatchQueueService batchQueueService, 
        IGenericService<Batch> batchService, 
        IGenericService<AssemblyLine> assemblyLineService, 
        IGenericService<Lathe> latheService, 
        IGenericService<Milling> millingService, 
        IGenericService<TestLine> testLineService,
        ProductionMonitoringContext context
        )
    {
        _batchQueueService = batchQueueService;
        _batchService = batchService;
        _assemblyLineService = assemblyLineService;
        _latheService = latheService;
        _millingService = millingService;
        _testLineService = testLineService;
        _context = context;
    }

    // private async Task<Batch?> GetBatch()
    // {
    //     var firstBatchId = await _batchQueueService.GetFirstBatchUuidAsync();
    //
    //     if (!firstBatchId.HasValue)
    //     {
    //         Console.WriteLine("Batch queue is empty. No batch in production.");
    //         // Non c'è nessun batch da elaborare. Restituisci null o un batch di default
    //         return null; // Indica che nessun batch è attualmente in produzione
    //     }
    //
    //     // Cerchiamo il batch nel database usando l'ID dalla coda
    //     var batch = await _batchService.GetByIdAsync(firstBatchId.Value);
    //
    //     // Gestiamo il caso in cui il batch non sia stato trovato nel database
    //     if (batch == null)
    //     {
    //         Console.WriteLine($"Batch with ID {firstBatchId.Value} not found in database. Removing from queue.");
    //
    //         // Se il batch non esiste nel DB, lo rimuoviamo dalla coda per evitare cicli infiniti o errori futuri.
    //         await _batchQueueService.DequeueAsync(firstBatchId.Value);
    //
    //         // Richiamiamo GetBatch() per cercare il prossimo batch valido nella coda.
    //         return await GetBatch(); // Chiamata ricorsiva per trovare il prossimo batch
    //     }
    //
    //     // A questo punto, 'batch' è garantito non essere null.
    //     // Ora puoi controllare le sue proprietà.
    //
    //     if (batch.ItemProduced < batch.ItemQuantity && !batch.isCompleted)
    //     {
    //         // Il batch è ancora in produzione e non è completato. Lo restituiamo.
    //         return batch;
    //     }
    //     // Il batch è già completato o ha ItemProduced >= ItemQuantity.
    //     // Lo marchiamo come completato (se non lo è già) e lo rimuoviamo dalla coda.
    //
    //     batch.isCompleted = true;
    //     await _batchService.UpdateAsync(batch); // Aggiorna lo stato nel DB
    //     Console.WriteLine($"Batch {batch.Id} marked as completed. And removed from queue.");
    //     await _batchQueueService.DequeueAsync(batch.Id); // Rimuovi il batch completato dalla coda.
    //     // Cerchiamo il prossimo batch valido nella coda.
    //     return await GetBatch(); // Chiamata ricorsiva
    //     
    // }
    private async Task<Batch?> GetBatch()
    {
        var batch = await _context.Batches.FirstOrDefaultAsync(b => b.isCompleted == false);
        if (batch == null) return null;
        
        if (batch.ItemProduced < batch.ItemQuantity)
            return batch;
        else
        {
            batch.isCompleted = true;
            await _context.SaveChangesAsync();
        }
        return await GetBatch();
    }

    public async Task ProcessTelemetryMessage(ReceivedData message)
    {
        // troviamo il batch su cui lavorare    (previously called workingBatch)
        Batch? inProductionBatch = await GetBatch();
        Console.WriteLine("chiamata 1");

        if (inProductionBatch == null)
        {
            throw new Exception("DIOC il batch è null, la funzione GetBatch non va un cazz.");
        }


        // Se siamo arrivati fino a qui, 'inProductionBatch' non è null, quindi possiamo procedere
        var assemblyLine = ProcessAssemblyLine(message, inProductionBatch.Id, inProductionBatch.SiteId);
        var lathe = ProcessLathe(message, inProductionBatch.Id, inProductionBatch.SiteId);
        var milling = ProcessMilling(message, inProductionBatch.Id, inProductionBatch.SiteId);
        var testLine = ProcessTestLine(message, inProductionBatch.Id, inProductionBatch.SiteId);


        // Salva i dati di produzione nel database.
        // Controlla che le istanze restituite da ProcessX siano effettivamente non null prima di inserire.
        if (assemblyLine != null)
        {
            await _assemblyLineService.InsertAsync(assemblyLine);
            Console.WriteLine($"AssemblyLine inserted for Batch {inProductionBatch.Id}.");
        }

        if (lathe != null)
        {
            await _latheService.InsertAsync(lathe);
            Console.WriteLine($"Lathe inserted for Batch {inProductionBatch.Id}.");
        }

        if (milling != null)
        {
            await _millingService.InsertAsync(milling);
            Console.WriteLine($"Milling inserted for Batch {inProductionBatch.Id}.");
        }

        if (testLine != null)
        {
            await _testLineService.InsertAsync(testLine);
            Console.WriteLine($"TestLine inserted for Batch {inProductionBatch.Id}.");
        }

        inProductionBatch = await CheckBatch(inProductionBatch);

        // Se CheckBatch ha completato il batch precedente e restituito un nuovo batch (o null se la coda è finita), qui puoi loggarlo o fare ulteriori azioni.
        Console.WriteLine($"Batch {inProductionBatch?.Id} updated/checked.");

        //if (inProductionBatch != null)
        //{
        //    inProductionBatch.ItemProduced += /* valore basato su telemetria */;
        //    if (inProductionBatch.ItemProduced >= inProductionBatch.ItemQuantity)
        //    {
        //        inProductionBatch.isCompleted = true;
        //    }
        //    await _batchService.UpdateAsync(inProductionBatch);
        //}

        // completiamo le istanze 
        //if (inProductionBatch != null)
        //{
        //    assemblyLine = ProcessAssemblyLine(message);
        //    lathe = ProcessLathe(message);
        //    milling = ProcessMilling(message);
        //    testLine = ProcessTestLine(message);

        //    inProductionBatch = await CheckBatch(inProductionBatch);
        //    Console.WriteLine("chiamata 2");
        //}


        //if (assemblyLine != null)
        //    await _assemblyLineService.InsertAsync(assemblyLine);
        //Console.WriteLine("chiamata 3");


        //if (lathe != null)
        //    await _latheService.InsertAsync(lathe);
        //Console.WriteLine("chiamata 4");


        //if (milling != null)
        //    await _millingService.InsertAsync(milling);
        //Console.WriteLine("chiamata 5");


        //if (testLine != null)
        //    await _testLineService.InsertAsync(testLine);
        //Console.WriteLine("chiamata 6");

        //  scrittura dei log su file
        string logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        Directory.CreateDirectory(logPath);

        string logFile = Path.Combine(logPath, $"association_log_{DateTime.Now:yyyy-MM-dd}.txt");
        string logEntry1 = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Milling associato:\n{milling}\n";
        string logEntry2 = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Lathe associato:\n{lathe}\n";
        string logEntry3 = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] AssemblyLine associato:\n{assemblyLine}\n";
        string logEntry4 = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] TestLine associato:\n{testLine}\n\n";

        StringBuilder sb = new StringBuilder();
        sb.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Processing telemetry for Batch ID: {inProductionBatch?.Id}\n");
        if (milling != null) sb.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Milling associato:\n{JsonSerializer.Serialize(milling)}\n"); // Uso JsonSerializer per un output leggibile
        if (lathe != null) sb.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Lathe associato:\n{JsonSerializer.Serialize(lathe)}\n");
        if (assemblyLine != null) sb.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] AssemblyLine associato:\n{JsonSerializer.Serialize(assemblyLine)}\n");
        if (testLine != null) sb.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] TestLine associato:\n{JsonSerializer.Serialize(testLine)}\n\n");

        await File.AppendAllTextAsync(logFile, sb.ToString());
        Console.WriteLine($"Telemetry processed and logged for Batch {inProductionBatch?.Id}."); // Log finale
    }



    private AssemblyLine? ProcessAssemblyLine(ReceivedData message, Guid batchId, Guid siteId)
    {
        if (message?.AssemblyLine == null) return null;

        // TODO: per tutte le istanze capire se serve o meno mantenere i campi isFirst e isLast
        var assemblyLine = new AssemblyLine 
        {
            Id = Guid.NewGuid(),
            BatchId = batchId,
            AverageStationTime = message.AssemblyLine.AverageStationTime,
            OperatorsNumber = message.AssemblyLine.OperatorsNumber,
            Faults = message.AssemblyLine.Faults,
            SiteId = siteId,
            TimestampLocal = message.AssemblyLine.TimestampLocal,
            TimestampUtc = message.AssemblyLine.TimestampUtc,
            MachineBlockage = message.AssemblyLine.MachineBlockage,
            BlockageCause = message.AssemblyLine.BlockageCause,
            LastMaintenance = message.AssemblyLine.LastMaintenance
        };

        return assemblyLine;
    }

    private Lathe? ProcessLathe(ReceivedData message, Guid batchId, Guid siteId)
    {
        if (message?.Lathe == null) return null;

        var lathe = new Lathe {
            Id = Guid.NewGuid(),
            BatchId = batchId,
            MachineState = message.Lathe.MachineState,
            Rpm = message.Lathe.Rpm,
            SpindleTemperature = message.Lathe.SpindleTemperature,
            CompletedItemsQuantity = message.Lathe.CompletedItemsQuantity,
            SiteId = siteId,
            TimestampLocal = message.Lathe.TimestampLocal,
            TimestampUtc = message.Lathe.TimestampUtc,
            MachineBlockage = message.Lathe.MachineBlockage,
            BlockageCause = message.Lathe.BlockageCause,
            LastMaintenance = message.Lathe.LastMaintenance
        };

        return lathe;
    }

    private Milling? ProcessMilling(ReceivedData message, Guid batchId, Guid siteId)
    {
        if (message?.Milling == null) return null;

        var milling = new Milling {
            Id = Guid.NewGuid(),
            BatchId = batchId,
            CycleDuration = message.Milling.CycleDuration,
            CuttingDepth = message.Milling.CuttingDepth,
            Vibration = message.Milling.Vibration,
            UserAlerts = message.Milling.UserAlerts,
            SiteId = siteId,
            TimestampLocal = message.Milling.TimestampLocal,
            TimestampUtc = message.Milling.TimestampUtc,
            MachineBlockage = message.Milling.MachineBlockage,
            BlockageCause = message.Milling.BlockageCause,
            LastMaintenance = message.Milling.LastMaintenance
        };

        return milling;
    }

    private TestLine? ProcessTestLine(ReceivedData message, Guid batchId, Guid siteId)
    {
        if (message?.TestLine == null) return null;

        var testLine = new TestLine {
            Id = Guid.NewGuid(),
            BatchId = batchId,
            TestResult = message.TestLine.TestResult,
            BoilerPressure = message.TestLine.BoilerPressure,
            BoilerTemperature = message.TestLine.BoilerTemperature,
            EnergyConsumption = message.TestLine.EnergyConsumption,
            SiteId = siteId,
            TimestampLocal = message.TestLine.TimestampLocal,
            TimestampUtc = message.TestLine.TimestampUtc,
            MachineBlockage = message.TestLine.MachineBlockage,
            BlockageCause = message.TestLine.BlockageCause,
            LastMaintenance = message.TestLine.LastMaintenance
        };

        return testLine;
    }

    // TODO: capire se posso cancellare o meno il fatto che deve ritornare il batch, essendo globale potrebbe non servire ritornare, fare test
    private async Task<Batch?> CheckBatch(Batch inProductionBatch)   //  previously called workBatch
    {
        inProductionBatch.ItemProduced = (inProductionBatch.ItemProduced ?? 0) + 1;

        if (inProductionBatch.ItemProduced >= inProductionBatch.ItemQuantity)
        {
            inProductionBatch.isCompleted = true;
            await _batchService.UpdateAsync(inProductionBatch);
            Console.WriteLine($"Batch {inProductionBatch.Id} completed and updated in DB.");

            var nextBatch = await GetBatch();
            Console.WriteLine($"Batch {inProductionBatch.Id} ItemProduced incremented to {inProductionBatch.ItemProduced}.");
        }

        return inProductionBatch;
    }
}