using PW2_Gruppo3.DataGenerator;
using PW2_Gruppo3.DataGenerator.Template;
using PW2_Gruppo3.Models;
using System.Collections.Concurrent;
using AssemblyLine = PW2_Gruppo3.Models.AssemblyLine;
using Lathe = PW2_Gruppo3.Models.Lathe;
using Milling = PW2_Gruppo3.Models.Milling;
using TestLine = PW2_Gruppo3.Models.TestLine;


namespace PW2_Gruppo3.ApiService.Services;

//public class BatchAssociationService
//{
//    public AssemblyLine? ProcessAssemblyLine(Message message)
//    {
//        if (message?.AssemblyLine == null) return null;

//        var assemblyLine = new AssemblyLine
//        {
//            Machine = message.AssemblyLine.Machine,
//            AverageStationTime = message.AssemblyLine.AverageStationTime,
//            OperatorsNumber = message.AssemblyLine.OperatorsNumber,
//            Faults = message.AssemblyLine.Faults,
//            Site = message.AssemblyLine.Site,
//            TimeStampLocal = message.AssemblyLine.TimeStampLocal,
//            TimeStampUtc = message.AssemblyLine.TimeStampUtc,
//            MachineBlockage = message.AssemblyLine.MachineBlockage,
//            BlockageCause = message.AssemblyLine.BlockageCause,
//            LastMaintenance = message.AssemblyLine.LastMaintenance
//        };

//        return assemblyLine;
//    }

//    public Lathe? ProcessLathe(Message message)
//    {
//        if (message?.Lathe == null) return null;

//        var lathe = new Lathe
//        {
//            Machine = message.Lathe.Machine,
//            MachineState = message.Lathe.MachineState,
//            RotationSpeed = message.Lathe.RotationSpeed,
//            SpindleTemperature = message.Lathe.SpindleTemperature,
//            CompletedItems = message.Lathe.CompletedItems,
//            Site = message.Lathe.Site,
//            TimeStampLocal = message.Lathe.TimeStampLocal,
//            TimeStampUtc = message.Lathe.TimeStampUtc,
//            MachineBlockage = message.Lathe.MachineBlockage,
//            BlockageCause = message.Lathe.BlockageCause,
//            LastMaintenance = message.Lathe.LastMaintenance
//        };

//        return lathe;
//    }

//    public Milling? ProcessMilling(Message message)
//    {
//        if (message?.Milling == null) return null;

//        var milling = new Milling
//        {
//            Machine = message.Milling.Machine,
//            CycleTime = message.Milling.CycleTime,
//            CuttingDepth = message.Milling.CuttingDepth,
//            Vibration = message.Milling.Vibration,
//            UserAlerts = message.Milling.UserAlerts,
//            Site = message.Milling.Site,
//            TimeStampLocal = message.Milling.TimeStampLocal,
//            TimeStampUtc = message.Milling.TimeStampUtc,
//            MachineBlockage = message.Milling.MachineBlockage,
//            BlockageCause = message.Milling.BlockageCause,
//            LastMaintenance = message.Milling.LastMaintenance
//        };

//        return milling;
//    }

//    public TestLine? ProcessTestLine(Message message)
//    {
//        if (message?.TestLine == null) return null;

//        var testLine = new TestLine
//        {
//            Machine = message.TestLine.Machine,
//            TestResult = message.TestLine.TestResult,
//            BoilerPressure = message.TestLine.BoilerPressure,
//            BoilerTemperature = message.TestLine.BoilerTemperature,
//            EnergyConsumption = message.TestLine.EnergyConsumption,
//            Site = message.TestLine.Site,
//            TimeStampLocal = message.TestLine.TimeStampLocal,
//            TimeStampUtc = message.TestLine.TimeStampUtc,
//            MachineBlockage = message.TestLine.MachineBlockage,
//            BlockageCause = message.TestLine.BlockageCause,
//            LastMaintenance = message.TestLine.LastMaintenance
//        };

//        return testLine;
//    }

//    public void ProcessTelemetryMessage(Message message)
//    {
//        // Questo metodo è utile se vuoi chiamarne solo uno
//        var assemblyLine = ProcessAssemblyLine(message);
//        var lathe = ProcessLathe(message);
//        var milling = ProcessMilling(message);
//        var testLine = ProcessTestLine(message);

//        // Qui puoi eventualmente:  (ma non ci interessano al momento, se non il terzultimo)
//        // - salvare i dati su DB
//        // - inviare eventi
//        // - aggiornare lo stato di un lotto
//        // - loggare in modo centralizzato

//        Console.WriteLine("Telemetry elaborata con successo.");
//    }
//}


// Strutture dati per il pipeline tracking
public class PipelineItem
{
    public Guid TempId { get; set; } = Guid.NewGuid();
    public string Site { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Dati delle 4 fasi (null se non ancora processata)
    public Milling? MillingData { get; set; }
    public Lathe? LatheData { get; set; }
    public AssemblyLine? AssemblyData { get; set; }
    public TestLine? TestData { get; set; }

    // Stato
    public bool IsComplete => TestData != null;
    public bool IsSuccessful => TestData?.TestResult == "OK";
    public int PhaseCount =>
        (MillingData != null ? 1 : 0) +
        (LatheData != null ? 1 : 0) +
        (AssemblyData != null ? 1 : 0) +
        (TestData != null ? 1 : 0);
}

public class SiteProductionState
{
    public string Site { get; set; } = string.Empty;
    public Guid? CurrentBatchId { get; set; }
    public int BatchTargetQuantity { get; set; }
    public int CompletedPieces { get; set; }
    public int StartedPieces { get; set; }  //  ?? o diamo per scontato che ogni pezzo iniziato venga anche ultimato, per semplicità?
    public Queue<PipelineItem> Pipeline { get; set; } = new();

    public bool IsBatchComplete => CompletedPieces >= BatchTargetQuantity;
    public bool ShouldStartNewBatch => CurrentBatchId == null;
    //  se a true => nessun batch attualmente in produzione, iniziamone tranquillamente uno nuovo
    //  se a false => un batch sta già venendo lavorato (quello che valorizza la prop CurrentBatchId)
}

public class BatchAssociationService
{
    private readonly IBatchQueueService _batchQueueService;
    private readonly IGenericService<Batch> _batchService;
    private readonly IGenericService<Milling> _millingService;
    private readonly IGenericService<Lathe> _latheService;
    private readonly IGenericService<AssemblyLine> _assemblyService;
    private readonly IGenericService<TestLine> _testService;

    // Thread-safe dictionary per gestire stati multipli per sito
    private readonly ConcurrentDictionary<string, SiteProductionState> _siteStates = new();

    public BatchAssociationService(
        IBatchQueueService batchQueueService,
        IGenericService<Batch> batchService,
        IGenericService<Milling> millingService,
        IGenericService<Lathe> latheService,
        IGenericService<AssemblyLine> assemblyService,
        IGenericService<TestLine> testService)
    {
        _batchQueueService = batchQueueService;
        _batchService = batchService;
        _millingService = millingService;
        _latheService = latheService;
        _assemblyService = assemblyService;
        _testService = testService;
    }

    public async Task ProcessTelemetryMessageAsync(Message message)
    {
        // Processa ogni tipo di macchina se presente nel messaggio
        if (message.Milling != null)
            await ProcessMillingDataAsync(message.Milling);

        if (message.Lathe != null)
            await ProcessLatheDataAsync(message.Lathe);

        if (message.AssemblyLine != null)
            await ProcessAssemblyDataAsync(message.AssemblyLine);

        if (message.TestLine != null)
            await ProcessTestDataAsync(message.TestLine);
    }

    private async Task ProcessMillingDataAsync(DataGenerator.Template.Milling millingTemplate)
    {
        var site = millingTemplate.Site;
        var siteState = GetOrCreateSiteState(site);

        // Se non c'è un lotto attivo, ne prendiamo uno dalla coda
        if (siteState.ShouldStartNewBatch)
        {
            await StartNewBatchForSiteAsync(siteState);
        }

        // Crea nuovo PipelineItem per questo pezzo
        var pipelineItem = new PipelineItem
        {
            Site = site,
            MillingData = CreateMillingFromTemplate(millingTemplate, siteState.CurrentBatchId!.Value)
            //  non capisco perchè mi dia fastidi visto che come param di ProcessMillingDataAsync gli passo Milling il template (di DataGenerator) e non Milling il model
        };

        // Imposta IsFirst per il primo pezzo del lotto
        pipelineItem.MillingData.IsFirst = (siteState.StartedPieces == 0);

        // Aggiungi alla pipeline
        siteState.Pipeline.Enqueue(pipelineItem);
        siteState.StartedPieces++;

        Console.WriteLine($"[MILLING] Nuovo pezzo iniziato per lotto {siteState.CurrentBatchId} nel sito {site}");
    }

    private async Task ProcessLatheDataAsync(DataGenerator.Template.Lathe latheTemplate)
    {
        var site = latheTemplate.Site;
        var siteState = GetOrCreateSiteState(site);

        // Trova il primo PipelineItem senza dati del tornio
        var pipelineItem = siteState.Pipeline.FirstOrDefault(p => p.Site == site && p.LatheData == null);

        if (pipelineItem != null)
        {
            pipelineItem.LatheData = CreateLatheFromTemplate(latheTemplate, siteState.CurrentBatchId!.Value);
            Console.WriteLine($"[LATHE] Pezzo processato dal tornio per lotto {siteState.CurrentBatchId} nel sito {site}");
        }
        else
        {
            Console.WriteLine($"[WARNING] Dati tornio ricevuti ma nessun pezzo in attesa nel sito {site}");
        }
    }

    private async Task ProcessAssemblyDataAsync(DataGenerator.Template.AssemblyLine assemblyTemplate)
    {
        var site = assemblyTemplate.Site;
        var siteState = GetOrCreateSiteState(site);

        // Trova il primo PipelineItem senza dati di assemblaggio
        var pipelineItem = siteState.Pipeline.FirstOrDefault(p => p.Site == site && p.AssemblyData == null);

        if (pipelineItem != null)
        {
            pipelineItem.AssemblyData = CreateAssemblyFromTemplate(assemblyTemplate, siteState.CurrentBatchId!.Value);
            Console.WriteLine($"[ASSEMBLY] Pezzo assemblato per lotto {siteState.CurrentBatchId} nel sito {site}");
        }
        else
        {
            Console.WriteLine($"[WARNING] Dati assembly ricevuti ma nessun pezzo in attesa nel sito {site}");
        }
    }

    private async Task ProcessTestDataAsync(DataGenerator.Template.TestLine testTemplate)
    {
        var site = testTemplate.Site;
        var siteState = GetOrCreateSiteState(site);

        // Trova il primo PipelineItem senza dati di test
        var pipelineItem = siteState.Pipeline.FirstOrDefault(p => p.Site == site && p.TestData == null);

        if (pipelineItem != null)
        {
            pipelineItem.TestData = CreateTestFromTemplate(testTemplate, siteState.CurrentBatchId!.Value);

            // Imposta IsLast per l'ultimo pezzo del lotto
            var willCompleteBatch = siteState.CompletedPieces + 1 >= siteState.BatchTargetQuantity;
            pipelineItem.TestData.IsLast = willCompleteBatch;

            Console.WriteLine($"[TEST] Pezzo testato per lotto {siteState.CurrentBatchId} nel sito {site} - Risultato: {testTemplate.TestResult}");

            // Salva il pezzo completato su DB
            await SaveCompletedPipelineItemAsync(pipelineItem);

            // Aggiorna contatori
            if (pipelineItem.IsSuccessful)
            {
                siteState.CompletedPieces++;
            }

            // Rimuovi dalla pipeline
            var tempQueue = new Queue<PipelineItem>();
            while (siteState.Pipeline.Count > 0)
            {
                var item = siteState.Pipeline.Dequeue();
                if (item.TempId != pipelineItem.TempId)
                    tempQueue.Enqueue(item);
            }
            siteState.Pipeline = tempQueue;

            // Controlla se il lotto è completato
            if (siteState.IsBatchComplete)
            {
                await CompleteBatchAsync(siteState);
            }
        }
        else
        {
            Console.WriteLine($"[WARNING] Dati test ricevuti ma nessun pezzo in attesa nel sito {site}");
        }
    }

    private async Task StartNewBatchForSiteAsync(SiteProductionState siteState)
    {
        var nextBatchId = await _batchQueueService.DequeueAsync();
        if (nextBatchId.HasValue)
        {
            var batch = await _batchService.GetByIdAsync(nextBatchId.Value);
            if (batch != null)
            {
                siteState.CurrentBatchId = nextBatchId.Value;
                siteState.BatchTargetQuantity = batch.ItemQuantity;
                siteState.CompletedPieces = 0;
                siteState.StartedPieces = 0;

                Console.WriteLine($"[BATCH] Iniziato nuovo lotto {nextBatchId} nel sito {siteState.Site} - Target: {batch.ItemQuantity} pezzi");
            }
        }
        else
        {
            Console.WriteLine($"[WARNING] Nessun lotto in coda per il sito {siteState.Site}");
        }
    }

    private static async Task CompleteBatchAsync(SiteProductionState siteState)
    {
        Console.WriteLine($"[BATCH] Completato lotto {siteState.CurrentBatchId} nel sito {siteState.Site} - Pezzi prodotti: {siteState.CompletedPieces}");

        // Reset dello stato per il prossimo lotto
        siteState.CurrentBatchId = null;
        siteState.BatchTargetQuantity = 0;
        siteState.CompletedPieces = 0;
        siteState.StartedPieces = 0;
        siteState.Pipeline.Clear();
    }

    private async Task SaveCompletedPipelineItemAsync(PipelineItem item)
    {
        // Salva tutti i dati delle macchine su DB
        if (item.MillingData != null)
            await _millingService.InsertAsync(item.MillingData);

        if (item.LatheData != null)
            await _latheService.InsertAsync(item.LatheData);

        if (item.AssemblyData != null)
            await _assemblyService.InsertAsync(item.AssemblyData);

        if (item.TestData != null)
            await _testService.InsertAsync(item.TestData);
    }

    private SiteProductionState GetOrCreateSiteState(string site)
    {
        return _siteStates.GetOrAdd(site, _ => new SiteProductionState { Site = site });
    }

    // Metodi helper per creare le entità a partire dei template
    //  boh non so, un bordello tra models e templates
    private Milling CreateMillingFromTemplate(DataGenerator.Template.Milling millingTemplate, Guid batchId)
    {
        return new Milling
        {
            Id = Guid.NewGuid(),
            ItemId = Guid.NewGuid(), // Genera un ID temporaneo per il pezzo
            BatchId = batchId,
            Machine = millingTemplate.Machine,
            CycleTime = millingTemplate.CycleTime,
            CuttingDepth = millingTemplate.CuttingDepth,
            Vibration = millingTemplate.Vibration,
            UserAlerts = millingTemplate.UserAlerts,
            Site = millingTemplate.Site,
            TimestampLocal = millingTemplate.TimeStampLocal,
            TimestampUtc = millingTemplate.TimeStampUtc,
            MachineBlockage = millingTemplate.MachineBlockage,
            BlockageCause = millingTemplate.BlockageCause,
            LastMaintenance = millingTemplate.LastMaintenance
        };
    }

    private Lathe CreateLatheFromTemplate(DataGenerator.Template.Lathe latheTemplate, Guid batchId)
    {
        return new Lathe
        {
            Id = Guid.NewGuid(),
            ItemId = Guid.NewGuid(),
            BatchId = batchId,
            Machine = latheTemplate.Machine,
            MachineState = latheTemplate.MachineState,
            RotationSpeed = latheTemplate.RotationSpeed,
            SpindleTemperature = latheTemplate.SpindleTemperature,
            CompletedItems = latheTemplate.CompletedItems,
            Site = latheTemplate.Site,
            TimestampLocal = latheTemplate.TimeStampLocal,
            TimestampUtc = latheTemplate.TimeStampUtc,
            MachineBlockage = latheTemplate.MachineBlockage,
            BlockageCause = latheTemplate.BlockageCause,
            LastMaintenance = latheTemplate.LastMaintenance
        };
    }

    private AssemblyLine CreateAssemblyFromTemplate(DataGenerator.Template.AssemblyLine assemblyLineTemplate, Guid batchId)
    {
        return new AssemblyLine
        {
            Id = Guid.NewGuid(),
            ItemId = Guid.NewGuid(),
            BatchId = batchId,
            Machine = assemblyLineTemplate.Machine,
            AverageStationTime = assemblyLineTemplate.AverageStationTime,
            OperatorsNumber = assemblyLineTemplate.OperatorsNumber,
            Faults = assemblyLineTemplate.Faults,
            Site = assemblyLineTemplate.Site,
            TimestampLocal = assemblyLineTemplate.TimeStampLocal,
            TimestampUtc = assemblyLineTemplate.TimeStampUtc,
            MachineBlockage = assemblyLineTemplate.MachineBlockage,
            BlockageCause = assemblyLineTemplate.BlockageCause,
            LastMaintenance = assemblyLineTemplate.LastMaintenance
        };
    }

    private TestLine CreateTestFromTemplate(TestLine testLineTemplate, Guid batchId)
    {
        return new TestLine
        {
            Id = Guid.NewGuid(),
            ItemId = Guid.NewGuid(),
            BatchId = batchId,
            Machine = testLineTemplate.Machine,
            TestResult = testLineTemplate.TestResult,
            BoilerPressure = testLineTemplate.BoilerPressure,
            BoilerTemperature = testLineTemplate.BoilerTemperature,
            EnergyConsumption = testLineTemplate.EnergyConsumption,
            Site = testLineTemplate.Site,
            TimestampLocal = testLineTemplate.TimeStampLocal,
            TimestampUtc = testLineTemplate.TimeStampUtc,
            MachineBlockage = testLineTemplate.MachineBlockage,
            BlockageCause = testLineTemplate.BlockageCause,
            LastMaintenance = testLineTemplate.LastMaintenance
        };
    }

    // Metodo di backward compatibility (deprecato)
    [Obsolete("Usare ProcessTelemetryMessageAsync invece")]
    public void ProcessTelemetryMessage(Message message)
    {
        // Per backward compatibility, ma preferibile usare la versione async
        _ = ProcessTelemetryMessageAsync(message);
    }
}