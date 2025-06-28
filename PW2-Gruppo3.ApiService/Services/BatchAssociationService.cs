using System.Text;
using System.Text.Json;
using PW2_Gruppo3.ApiService.Data;
using PW2_Gruppo3.DataGenerator;
using PW2_Gruppo3.Models;


namespace PW2_Gruppo3.ApiService.Services;

public class BatchAssociationService
{
    private readonly IBatchQueueService _batchQueueService;
    private readonly IGenericService<Batch> _batchService;
    private readonly IGenericService<AssemblyLine> _alService;
    private readonly IGenericService<Lathe> _latheService;
    private readonly IGenericService<Milling> _millingService;
    private readonly IGenericService<TestLine> _tlService;
    private Batch inProductionBatch;

    public BatchAssociationService(IBatchQueueService batchQueueService, IGenericService<Batch> batchService, IGenericService<AssemblyLine> alService, IGenericService<Lathe> latheService, IGenericService<Milling> millingService, IGenericService<TestLine> tlService)
    {
        _batchQueueService = batchQueueService;
        _batchService = batchService;
        _alService = alService;
        _latheService = latheService;
        _millingService = millingService;
        _tlService = tlService;
    }

    private AssemblyLine? ProcessAssemblyLine(ReceivedData message)
    {
        if (message?.AssemblyLine == null) return null;

        // TODO: per tutte le istanze capire se serve o meno mantenere i campi isFirst e isLast
        var al = new AssemblyLine();
        al.Id = Guid.NewGuid();
        al.BatchId = inProductionBatch.Id;
        al.AverageStationTime = message.AssemblyLine.AverageStationTime;
        al.OperatorsNumber = message.AssemblyLine.OperatorsNumber;
        al.Faults = message.AssemblyLine.Faults;
        al.SiteId = message.AssemblyLine.SiteId;
        al.TimestampLocal = message.AssemblyLine.TimestampLocal;
        al.TimestampUtc = message.AssemblyLine.TimestampUtc;
        al.MachineBlockage = message.AssemblyLine.MachineBlockage;
        al.BlockageCause = message.AssemblyLine.BlockageCause;
        al.LastMaintenance = message.AssemblyLine.LastMaintenance;

        return al;
    }

    private Lathe? ProcessLathe(ReceivedData message)
    {
        if (message?.Lathe == null) return null;

        var lathe = new Lathe();
        lathe.Id = Guid.NewGuid();
        lathe.BatchId = inProductionBatch.Id;
        lathe.MachineState = message.Lathe.MachineState;
        lathe.Rpm = message.Lathe.Rpm;
        lathe.SpindleTemperature = message.Lathe.SpindleTemperature;
        lathe.CompletedItemsQuantity = message.Lathe.CompletedItemsQuantity;
        lathe.SiteId = message.Lathe.SiteId;
        lathe.TimestampLocal = message.Lathe.TimestampLocal;
        lathe.TimestampUtc = message.Lathe.TimestampUtc;
        lathe.MachineBlockage = message.Lathe.MachineBlockage;
        lathe.BlockageCause = message.Lathe.BlockageCause;
        lathe.LastMaintenance = message.Lathe.LastMaintenance;

        return lathe;
    }

    private Milling? ProcessMilling(ReceivedData message)
    {
        if (message?.Milling == null) return null;

        var milling = new Milling();
        milling.Id = Guid.NewGuid();
        milling.BatchId = inProductionBatch.Id;
        milling.CycleDuration = message.Milling.CycleDuration;
        milling.CuttingDepth = message.Milling.CuttingDepth;
        milling.Vibration = message.Milling.Vibration;
        milling.UserAlerts = message.Milling.UserAlerts;
        milling.SiteId = message.Milling.SiteId;
        milling.TimestampLocal = message.Milling.TimestampLocal;
        milling.TimestampUtc = message.Milling.TimestampUtc;
        milling.MachineBlockage = message.Milling.MachineBlockage;
        milling.BlockageCause = message.Milling.BlockageCause;
        milling.LastMaintenance = message.Milling.LastMaintenance;

        return milling;
    }

    private TestLine? ProcessTestLine(ReceivedData message)
    {
        if (message?.TestLine == null) return null;

        var tl = new TestLine();
        tl.Id = Guid.NewGuid();
        tl.BatchId = inProductionBatch.Id;
        tl.TestResult = message.TestLine.TestResult;
        tl.BoilerPressure = message.TestLine.BoilerPressure;
        tl.BoilerTemperature = message.TestLine.BoilerTemperature;
        tl.EnergyConsumption = message.TestLine.EnergyConsumption;
        tl.SiteId = message.TestLine.SiteId;
        tl.TimestampLocal = message.TestLine.TimestampLocal;
        tl.TimestampUtc = message.TestLine.TimestampUtc;
        tl.MachineBlockage = message.TestLine.MachineBlockage;
        tl.BlockageCause = message.TestLine.BlockageCause;
        tl.LastMaintenance = message.TestLine.LastMaintenance;

        return tl;
    }

    public async Task ProcessTelemetryMessage(ReceivedData message)
    {
        // troviamo il batch su cui lavorare    (previously called workingBatch)
        inProductionBatch = await GetBatch();

        if (inProductionBatch != null)
        {
            return;
        }

        var assemblyLine = new AssemblyLine();
        var lathe = new Lathe();
        var milling = new Milling();
        var testLine = new TestLine();


        // completiamo le istanze 
        if (inProductionBatch != null)
        {
            assemblyLine = ProcessAssemblyLine(message);
            lathe = ProcessLathe(message);
            milling = ProcessMilling(message);
            testLine = ProcessTestLine(message);

            inProductionBatch = await CheckBatch(inProductionBatch);
        }


        if (assemblyLine != null)
            await _alService.InsertAsync(assemblyLine);

        if (lathe != null)
            await _latheService.InsertAsync(lathe);

        if (milling != null)
            await _millingService.InsertAsync(milling);

        if (testLine != null)
            await _tlService.InsertAsync(testLine);

        string logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        Directory.CreateDirectory(logPath);

        string logFile = Path.Combine(logPath, $"association_log_{DateTime.Now:yyyy-MM-dd}.txt");
        string logEntry1 = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Milling associato:\n{milling}\n";
        string logEntry2 = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Lathe associato:\n{lathe}\n";
        string logEntry3 = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] AssemblyLine associato:\n{assemblyLine}\n";
        string logEntry4 = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] TestLine associato:\n{testLine}\n\n";

        StringBuilder sb = new StringBuilder();
        sb.Append(logEntry1);
        sb.Append(logEntry2);
        sb.Append(logEntry3);
        sb.Append(logEntry4);

        await File.AppendAllTextAsync(logFile, sb.ToString());
    }

    // TODO: capire se posso cancellare o meno il fatto che deve ritornare il batch, essendo globale potrebbe non servire ritornare, fare test
    private async Task<Batch> CheckBatch(Batch inProductionBatch)   //  previously called workBatch
    {
        inProductionBatch.ItemProduced++;

        if (inProductionBatch.ItemProduced >= inProductionBatch.ItemQuantity)
        {
            inProductionBatch.isCompleted = true;
            await _batchService.UpdateAsync(inProductionBatch);
            inProductionBatch = await GetBatch();
        }

        return inProductionBatch;
    }

    private async Task<Batch> GetBatch()
    {
        var firstBatch = await _batchQueueService.GetFirstBatchUuidAsync();
        var batch = new Batch();

        if (firstBatch.HasValue)
        {
            batch = await _batchService.GetByIdAsync(firstBatch.Value);

            if (batch.ItemProduced < batch.ItemQuantity)
            {
                if (!batch.isCompleted)
                    return batch;

                batch.isCompleted = true;
                await _batchService.UpdateAsync(batch);
            }
        }
        else
            return batch;


        // se a questo punto la funzione non ha ritornato vuol dire che il batch è completato ma
        // è ancora dentro la coda, quindi lo tolgo dalla coda e poi vado di ricorsione per 
        // trovare un batch non completo
        await _batchQueueService.DequeueAsync(batch.Id);
        return await GetBatch();
    }
}