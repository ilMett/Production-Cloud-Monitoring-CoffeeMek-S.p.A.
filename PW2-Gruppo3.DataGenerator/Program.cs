using System.Net.Http.Json;
using System.Text.Json;
using System.Timers;
using Bogus;   // Faker per generare i dati 
using PW2_Gruppo3.DataGenerator;
using PW2_Gruppo3.DataGenerator.Template;

namespace Pw2_Gruppo3.DataGenerator;

public class Program
{
    private static System.Timers.Timer _timer;
    private static int _generationCount = 0;

    public static async Task Main(string[] args)
    {
        var cts = new CancellationTokenSource();
        
        while (!cts.Token.IsCancellationRequested)
        {
            var data = GenerateMessageData();

            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync(
                "http://localhost:5580/api/v1/telemetry/machines",
                data,
                cts.Token
            );
            
            Console.WriteLine(response.IsSuccessStatusCode ? $"Dati inviati con successo alle {DateTime.Now}" : $"Errore durante l'invio: {response.StatusCode}");

            await Task.Delay(TimeSpan.FromSeconds(1000), cts.Token);
        }

    }
    
    private static Message GenerateMessageData()
    {
        Randomizer.Seed = new Random(); 
        var faker = new Faker("it");
        
        bool isBlocked = (_generationCount % 100 == 0); 
        
        var milling = new Milling()
        {
            Machine = "Fresa CNC",
            CycleTime = faker.Random.Decimal(10, 60).ToString("0.00"), 
            CuttingDepth = faker.Random.Decimal(1, 10).ToString("0.00"), 
            Vibration = faker.Random.Decimal(0.1m, 1.5m).ToString("0.000"), 
            UserAlerts = faker.PickRandom(new[] { "Nessun avviso", "Rottura utensile", "Sovratemperatura motore", "Mancanza di lubrificante", "Errore di programmazione" }),
            Site = faker.PickRandom(new[] {"Italia", "Vietnam", "Brasile"}),
            TimeStampLocal = faker.Date.Recent(5).ToString("yyyy-MM-ddTHH:mm:ss"), 
            TimeStampUtc = faker.Date.Recent(5).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"), 
            MachineBlockage = isBlocked.ToString(), 
            BlockageCause = isBlocked ? faker.PickRandom(new[] { "Manutenzione urgente", "Rottura materiale", "Errore operatore", "Sovraccarico", "Guasto sensore" }) : null, // Imposta BlockageCause solo se bloccato
            LastMaintenance = faker.Date.Past(2).ToString("yyyy-MM-ddTHH:mm:ss") 
        };

        var lathe = new Lathe()
        {
            Machine = "Tornio automatico",
            MachineState = faker.PickRandom(new[] { "Operativa", "Inattiva", "Manutenzione", "Guasto" }),
            RotationSpeed = faker.Random.Int(1000, 3000).ToString(), 
            SpindleTemperature = faker.Random.Decimal(30.0m, 60.0m).ToString("0.00"),
            CompletedItems = faker.Random.Int(10, 100).ToString(), 
            Site = faker.PickRandom(new[] {"Italia", "Vietnam", "Brasile"}),
            TimeStampLocal = faker.Date.Recent(5).ToString("yyyy-MM-ddTHH:mm:ss"),
            TimeStampUtc = faker.Date.Recent(5).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"),
            MachineBlockage = isBlocked.ToString(), 
            BlockageCause = isBlocked ? faker.PickRandom(new[] { "Surriscaldamento", "Rottura utensile", "Problema elettrico", "Mancanza materiale", "Pressione idraulica bassa" }) : null, // BlockageCause è null se non bloccato
            LastMaintenance = faker.Date.Past(2).ToString("yyyy-MM-ddTHH:mm:ss")
        };

        var assemblyLine = new AssemblyLine()
        {
            Machine = "Linea di Assemblaggio",
            AverageStationTime = faker.Random.Decimal(15.0m, 45.0m).ToString("0.00"),
            OperatorsNumber = faker.Random.Int(1, 5).ToString(), 
            Faults = faker.PickRandom(new[] { "Nessun difetto", "Componente mancante", "Errore di montaggio", "Difetto materiale", "Danno estetico" }),
            Site = faker.PickRandom(new[] {"Italia", "Vietnam", "Brasile"}),
            TimeStampLocal = faker.Date.Recent(5).ToString("yyyy-MM-ddTHH:mm:ss"),
            TimeStampUtc = faker.Date.Recent(5).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"),
            MachineBlockage = isBlocked.ToString(), 
            BlockageCause = isBlocked ? faker.PickRandom(new[] { "Errore software", "Guasto meccanico", "Interruzione alimentazione", "Mancanza componenti", "Problema di sicurezza" }) : null, // BlockageCause è null se non bloccato
            LastMaintenance = faker.Date.Past(2).ToString("yyyy-MM-ddTHH:mm:ss")
        };

        var testLine = new TestLine()
        {
            Machine = "Linea di Test",
            TestResult = faker.PickRandom(new[] { "OK", "FAIL", "PASS" }),
            BoilerPressure = faker.Random.Decimal(1.0m, 5.0m).ToString("0.00"), 
            BoilerTemperature = faker.Random.Decimal(80.0m, 110.0m).ToString("0.00"), 
            EnergyConsumption = faker.Random.Decimal(1.0m, 10.0m).ToString("0.00"),
            Site = faker.PickRandom(new[] {"Italia", "Vietnam", "Brasile"}),
            TimeStampLocal = faker.Date.Recent(5).ToString("yyyy-MM-ddTHH:mm:ss"),
            TimeStampUtc = faker.Date.Recent(5).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"),
            MachineBlockage = isBlocked.ToString(),
            BlockageCause = isBlocked ? faker.PickRandom(new[] { "Guasto meccanico", "Errore del sensore", "Mancanza di fluido", "Problema software", "Calibrazione errata" }) : null, // BlockageCause è null se non bloccato
            LastMaintenance = faker.Date.Past(2).ToString("yyyy-MM-ddTHH:mm:ss")
        };

        var message = new Message
        {
            Milling = milling,
            Lathe = lathe,
            AssemblyLine = assemblyLine,
            TestLine = testLine,
        };

        return message;
    }
}