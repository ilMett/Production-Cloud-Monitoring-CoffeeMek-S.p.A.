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

    //public BatchAssociationService(IBatchQueueService batchQueueService, IGenericService<Batch> batchService, IGenericService<AssemblyLine> alService, IGenericService<Lathe> latheService, IGenericService<Milling> millingService, IGenericService<TestLine> tlService)
    //{
    //    _batchQueueService = batchQueueService;
    //    _batchService = batchService;
    //    _alService = alService;
    //    _latheService = latheService;
    //    _millingService = millingService;
    //    _tlService = tlService;
    //}

    public AssemblyLine? ProcessAssemblyLine(ReceivedData message)
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

    public Lathe? ProcessLathe(ReceivedData message)
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

    public Milling? ProcessMilling(ReceivedData message)
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

    public TestLine? ProcessTestLine(ReceivedData message)
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
        
        // completiamo le istanze 
        var assemblyLine = ProcessAssemblyLine(message);
        var lathe = ProcessLathe(message);
        var milling = ProcessMilling(message);
        var testLine = ProcessTestLine(message);
        
        inProductionBatch = await CheckBatch(inProductionBatch);
        
        if (assemblyLine != null)
            await _alService.InsertAsync(assemblyLine);
        
        if (lathe != null)
            await _latheService.InsertAsync(lathe);
        
        if (milling != null)
            await _millingService.InsertAsync(milling);
        
        if (testLine != null)
            await _tlService.InsertAsync(testLine);

        Console.WriteLine("Telemetry elaborata con successo.");
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
            return null;

        
        // se a questo punto la funzione non ha ritornato vuol dire che il batch è completato ma
        // è ancora dentro la coda, quindi lo tolgo dalla coda e poi vado di ricorsione per 
        // trovare un batch non completo
        await _batchQueueService.DequeueAsync(batch.Id);
        return await GetBatch();
    }
}


// Strutture dati per il pipeline tracking
// public class PipelineItem
// {
//     public Guid TempId { get; set; } = Guid.NewGuid();
//     public string Site { get; set; } = string.Empty;
//     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//
//     // Dati delle 4 fasi (null se non ancora processata)
//     public Milling? MillingData { get; set; }
//     public Lathe? LatheData { get; set; }
//     public AssemblyLine? AssemblyData { get; set; }
//     public TestLine? TestData { get; set; }
//
//     // Stato
//     public bool IsComplete => TestData != null;
//     public bool IsSuccessful => TestData?.TestResult == "OK";
//     public int PhaseCount =>
//         (MillingData != null ? 1 : 0) +
//         (LatheData != null ? 1 : 0) +
//         (AssemblyData != null ? 1 : 0) +
//         (TestData != null ? 1 : 0);
// }
//
// public class SiteProductionState
// {
//     public string Site { get; set; } = string.Empty;
//     public Guid? CurrentBatchId { get; set; }
//     public int BatchTargetQuantity { get; set; }
//     public int CompletedPieces { get; set; }
//     public int StartedPieces { get; set; }  //  ?? o diamo per scontato che ogni pezzo iniziato venga anche ultimato, per semplicità?
//     public Queue<PipelineItem> Pipeline { get; set; } = new();
//
//     public bool IsBatchComplete => CompletedPieces >= BatchTargetQuantity;
//     public bool ShouldStartNewBatch => CurrentBatchId == null;
//     //  se a true => nessun batch attualmente in produzione, iniziamone tranquillamente uno nuovo
//     //  se a false => un batch sta già venendo lavorato (quello che valorizza la prop CurrentBatchId)
// }
//
// public class BatchAssociationService
// {
//     private readonly IBatchQueueService _batchQueueService;
//     private readonly IGenericService<Batch> _batchService;
//     private readonly IGenericService<Milling> _millingService;
//     private readonly IGenericService<Lathe> _latheService;
//     private readonly IGenericService<AssemblyLine> _assemblyService;
//     private readonly IGenericService<TestLine> _testService;
//
//     // Thread-safe dictionary per gestire stati multipli per sito
//     private readonly ConcurrentDictionary<string, SiteProductionState> _siteStates = new();
//
//     public BatchAssociationService(
//         IBatchQueueService batchQueueService,
//         IGenericService<Batch> batchService,
//         IGenericService<Milling> millingService,
//         IGenericService<Lathe> latheService,
//         IGenericService<AssemblyLine> assemblyService,
//         IGenericService<TestLine> testService)
//     {
//         _batchQueueService = batchQueueService;
//         _batchService = batchService;
//         _millingService = millingService;
//         _latheService = latheService;
//         _assemblyService = assemblyService;
//         _testService = testService;
//     }
//
//     public async Task ProcessTelemetryMessageAsync(Message message)
//     {
//         // Processa ogni tipo di macchina se presente nel messaggio
//         if (message.Milling != null)
//             await ProcessMillingDataAsync(message.Milling);
//
//         if (message.Lathe != null)
//             await ProcessLatheDataAsync(message.Lathe);
//
//         if (message.AssemblyLine != null)
//             await ProcessAssemblyDataAsync(message.AssemblyLine);
//
//         if (message.TestLine != null)
//             await ProcessTestDataAsync(message.TestLine);
//     }
//
//     private async Task ProcessMillingDataAsync(DataGenerator.Template.Milling millingTemplate)
//     {
//         var site = millingTemplate.Site;
//         var siteState = GetOrCreateSiteState(site);
//
//         // Se non c'è un lotto attivo, ne prendiamo uno dalla coda
//         if (siteState.ShouldStartNewBatch)
//         {
//             await StartNewBatchForSiteAsync(siteState);
//         }
//
//         // Crea nuovo PipelineItem per questo pezzo
//         var pipelineItem = new PipelineItem
//         {
//             Site = site,
//             MillingData = CreateMillingFromTemplate(millingTemplate, siteState.CurrentBatchId!.Value)
//             //  non capisco perchè mi dia fastidi visto che come param di ProcessMillingDataAsync gli passo Milling il template (di DataGenerator) e non Milling il model
//         };
//
//         // Imposta IsFirst per il primo pezzo del lotto
//         pipelineItem.MillingData.IsFirst = (siteState.StartedPieces == 0);
//
//         // Aggiungi alla pipeline
//         siteState.Pipeline.Enqueue(pipelineItem);
//         siteState.StartedPieces++;
//
//         Console.WriteLine($"[MILLING] Nuovo pezzo iniziato per lotto {siteState.CurrentBatchId} nel sito {site}");
//     }
//
//     private async Task ProcessLatheDataAsync(DataGenerator.Template.Lathe latheTemplate)
//     {
//         var site = latheTemplate.Site;
//         var siteState = GetOrCreateSiteState(site);
//
//         // Trova il primo PipelineItem senza dati del tornio
//         var pipelineItem = siteState.Pipeline.FirstOrDefault(p => p.Site == site && p.LatheData == null);
//
//         if (pipelineItem != null)
//         {
//             pipelineItem.LatheData = CreateLatheFromTemplate(latheTemplate, siteState.CurrentBatchId!.Value);
//             Console.WriteLine($"[LATHE] Pezzo processato dal tornio per lotto {siteState.CurrentBatchId} nel sito {site}");
//         }
//         else
//         {
//             Console.WriteLine($"[WARNING] Dati tornio ricevuti ma nessun pezzo in attesa nel sito {site}");
//         }
//     }
//
//     private async Task ProcessAssemblyDataAsync(DataGenerator.Template.AssemblyLine assemblyTemplate)
//     {
//         var site = assemblyTemplate.Site;
//         var siteState = GetOrCreateSiteState(site);
//
//         // Trova il primo PipelineItem senza dati di assemblaggio
//         var pipelineItem = siteState.Pipeline.FirstOrDefault(p => p.Site == site && p.AssemblyData == null);
//
//         if (pipelineItem != null)
//         {
//             pipelineItem.AssemblyData = CreateAssemblyFromTemplate(assemblyTemplate, siteState.CurrentBatchId!.Value);
//             Console.WriteLine($"[ASSEMBLY] Pezzo assemblato per lotto {siteState.CurrentBatchId} nel sito {site}");
//         }
//         else
//         {
//             Console.WriteLine($"[WARNING] Dati assembly ricevuti ma nessun pezzo in attesa nel sito {site}");
//         }
//     }
//
//     private async Task ProcessTestDataAsync(DataGenerator.Template.TestLine testTemplate)
//     {
//         var site = testTemplate.Site;
//         var siteState = GetOrCreateSiteState(site);
//
//         // Trova il primo PipelineItem senza dati di test
//         var pipelineItem = siteState.Pipeline.FirstOrDefault(p => p.Site == site && p.TestData == null);
//
//         if (pipelineItem != null)
//         {
//             pipelineItem.TestData = CreateTestFromTemplate(testTemplate, siteState.CurrentBatchId!.Value);
//
//             // Imposta IsLast per l'ultimo pezzo del lotto
//             var willCompleteBatch = siteState.CompletedPieces + 1 >= siteState.BatchTargetQuantity;
//             pipelineItem.TestData.IsLast = willCompleteBatch;
//
//             Console.WriteLine($"[TEST] Pezzo testato per lotto {siteState.CurrentBatchId} nel sito {site} - Risultato: {testTemplate.TestResult}");
//
//             // Salva il pezzo completato su DB
//             await SaveCompletedPipelineItemAsync(pipelineItem);
//
//             // Aggiorna contatori
//             if (pipelineItem.IsSuccessful)
//             {
//                 siteState.CompletedPieces++;
//             }
//
//             // Rimuovi dalla pipeline
//             var tempQueue = new Queue<PipelineItem>();
//             while (siteState.Pipeline.Count > 0)
//             {
//                 var item = siteState.Pipeline.Dequeue();
//                 if (item.TempId != pipelineItem.TempId)
//                     tempQueue.Enqueue(item);
//             }
//             siteState.Pipeline = tempQueue;
//
//             // Controlla se il lotto è completato
//             if (siteState.IsBatchComplete)
//             {
//                 await CompleteBatchAsync(siteState);
//             }
//         }
//         else
//         {
//             Console.WriteLine($"[WARNING] Dati test ricevuti ma nessun pezzo in attesa nel sito {site}");
//         }
//     }
//
//     private async Task StartNewBatchForSiteAsync(SiteProductionState siteState)
//     {
//         var nextBatchId = await _batchQueueService.DequeueAsync();
//         if (nextBatchId.HasValue)
//         {
//             var batch = await _batchService.GetByIdAsync(nextBatchId.Value);
//             if (batch != null)
//             {
//                 siteState.CurrentBatchId = nextBatchId.Value;
//                 siteState.BatchTargetQuantity = batch.ItemQuantity;
//                 siteState.CompletedPieces = 0;
//                 siteState.StartedPieces = 0;
//
//                 Console.WriteLine($"[BATCH] Iniziato nuovo lotto {nextBatchId} nel sito {siteState.Site} - Target: {batch.ItemQuantity} pezzi");
//             }
//         }
//         else
//         {
//             Console.WriteLine($"[WARNING] Nessun lotto in coda per il sito {siteState.Site}");
//         }
//     }
//
//     private static async Task CompleteBatchAsync(SiteProductionState siteState)
//     {
//         Console.WriteLine($"[BATCH] Completato lotto {siteState.CurrentBatchId} nel sito {siteState.Site} - Pezzi prodotti: {siteState.CompletedPieces}");
//
//         // Reset dello stato per il prossimo lotto
//         siteState.CurrentBatchId = null;
//         siteState.BatchTargetQuantity = 0;
//         siteState.CompletedPieces = 0;
//         siteState.StartedPieces = 0;
//         siteState.Pipeline.Clear();
//     }
//
//     private async Task SaveCompletedPipelineItemAsync(PipelineItem item)
//     {
//         // Salva tutti i dati delle macchine su DB
//         if (item.MillingData != null)
//             await _millingService.InsertAsync(item.MillingData);
//
//         if (item.LatheData != null)
//             await _latheService.InsertAsync(item.LatheData);
//
//         if (item.AssemblyData != null)
//             await _assemblyService.InsertAsync(item.AssemblyData);
//
//         if (item.TestData != null)
//             await _testService.InsertAsync(item.TestData);
//     }
//
//     private SiteProductionState GetOrCreateSiteState(string site)
//     {
//         return _siteStates.GetOrAdd(site, _ => new SiteProductionState { Site = site });
//     }
//
//     // Metodi helper per creare le entità a partire dei template
//     //  boh non so, un bordello tra models e templates
//     private Milling CreateMillingFromTemplate(DataGenerator.Template.Milling millingTemplate, Guid batchId)
//     {
//         return new Milling
//         {
//             Id = Guid.NewGuid(),
//             ItemId = Guid.NewGuid(), // Genera un ID temporaneo per il pezzo
//             BatchId = batchId,
//             Machine = millingTemplate.Machine,
//             CycleTime = millingTemplate.CycleTime,
//             CuttingDepth = millingTemplate.CuttingDepth,
//             Vibration = millingTemplate.Vibration,
//             UserAlerts = millingTemplate.UserAlerts,
//             Site = millingTemplate.Site,
//             TimestampLocal = millingTemplate.TimeStampLocal,
//             TimestampUtc = millingTemplate.TimeStampUtc,
//             MachineBlockage = millingTemplate.MachineBlockage,
//             BlockageCause = millingTemplate.BlockageCause,
//             LastMaintenance = millingTemplate.LastMaintenance
//         };
//     }
//
//     private Lathe CreateLatheFromTemplate(DataGenerator.Template.Lathe latheTemplate, Guid batchId)
//     {
//         return new Lathe
//         {
//             Id = Guid.NewGuid(),
//             ItemId = Guid.NewGuid(),
//             BatchId = batchId,
//             Machine = latheTemplate.Machine,
//             MachineState = latheTemplate.MachineState,
//             RotationSpeed = latheTemplate.RotationSpeed,
//             SpindleTemperature = latheTemplate.SpindleTemperature,
//             CompletedItems = latheTemplate.CompletedItems,
//             Site = latheTemplate.Site,
//             TimestampLocal = latheTemplate.TimeStampLocal,
//             TimestampUtc = latheTemplate.TimeStampUtc,
//             MachineBlockage = latheTemplate.MachineBlockage,
//             BlockageCause = latheTemplate.BlockageCause,
//             LastMaintenance = latheTemplate.LastMaintenance
//         };
//     }
//
//     private AssemblyLine CreateAssemblyFromTemplate(DataGenerator.Template.AssemblyLine assemblyLineTemplate, Guid batchId)
//     {
//         return new AssemblyLine
//         {
//             Id = Guid.NewGuid(),
//             ItemId = Guid.NewGuid(),
//             BatchId = batchId,
//             Machine = assemblyLineTemplate.Machine,
//             AverageStationTime = assemblyLineTemplate.AverageStationTime,
//             OperatorsNumber = assemblyLineTemplate.OperatorsNumber,
//             Faults = assemblyLineTemplate.Faults,
//             Site = assemblyLineTemplate.Site,
//             TimestampLocal = assemblyLineTemplate.TimeStampLocal,
//             TimestampUtc = assemblyLineTemplate.TimeStampUtc,
//             MachineBlockage = assemblyLineTemplate.MachineBlockage,
//             BlockageCause = assemblyLineTemplate.BlockageCause,
//             LastMaintenance = assemblyLineTemplate.LastMaintenance
//         };
//     }
//
//     private TestLine CreateTestFromTemplate(TestLine testLineTemplate, Guid batchId)
//     {
//         return new TestLine
//         {
//             Id = Guid.NewGuid(),
//             ItemId = Guid.NewGuid(),
//             BatchId = batchId,
//             Machine = testLineTemplate.Machine,
//             TestResult = testLineTemplate.TestResult,
//             BoilerPressure = testLineTemplate.BoilerPressure,
//             BoilerTemperature = testLineTemplate.BoilerTemperature,
//             EnergyConsumption = testLineTemplate.EnergyConsumption,
//             Site = testLineTemplate.Site,
//             TimestampLocal = testLineTemplate.TimeStampLocal,
//             TimestampUtc = testLineTemplate.TimeStampUtc,
//             MachineBlockage = testLineTemplate.MachineBlockage,
//             BlockageCause = testLineTemplate.BlockageCause,
//             LastMaintenance = testLineTemplate.LastMaintenance
//         };
//     }
//
//     // Metodo di backward compatibility (deprecato)
//     [Obsolete("Usare ProcessTelemetryMessageAsync invece")]
//     public void ProcessTelemetryMessage(Message message)
//     {
//         // Per backward compatibility, ma preferibile usare la versione async
//         _ = ProcessTelemetryMessageAsync(message);
//     }
// }