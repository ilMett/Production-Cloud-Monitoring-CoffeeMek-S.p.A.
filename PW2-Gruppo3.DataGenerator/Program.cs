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

        public static void Main(string[] args)
        {
            Console.WriteLine("Avvio della generazione dati pseudo-reali...");
            Console.WriteLine("La generazione avverrà ogni 30 secondi. Premi un tasto per terminare.");

            // Configura il timer
            _timer = new System.Timers.Timer(30000); 
            _timer.Elapsed += OnTimedEvent; 
            _timer.AutoReset = true; 
            _timer.Enabled = true; 

            // Esegui la prima generazione immediatamente
            OnTimedEvent(null, null);
            
            // premere un tasto per finire l'esecuzione del progetto
            Console.ReadKey();
            
            _timer.Stop();
            _timer.Dispose();
            Console.WriteLine("\nGenerazione dati terminata.");
        }


        // Metodo chiamato all'scadere dell'intervallo del timer
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            _generationCount++;
            Console.WriteLine($"\n--- Inizio Generazione Dati #{_generationCount} ({DateTime.Now}) ---");

            // Chiama la funzione per generare i dati e ottenere l'oggetto Message
            var message = GenerateMessageData();

            // Serializzazione della classe Message in formato JSON
            Console.WriteLine("\nSerializzazione della classe Message in JSON...");
            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(message, options);

            Console.WriteLine("\n--- Output JSON Generato ---");
            Console.WriteLine(jsonString);
            Console.WriteLine("--- Fine Generazione Dati ---");
        }
        
        private static Message GenerateMessageData()
        {
            // Configura Faker per la lingua italiana
            Randomizer.Seed = new Random(); // Per risultati riproducibili diversi ad ogni generazione
            var faker = new Faker("it");
            
            // TODO: usare il faker per generare i dati (capire con chat come fare)
            var milling = new Milling()
            {
                // TODO: generare dati causali del MILLING 
            };

            var lathe = new Lathe()
            {
                // TODO: generare dati causali del LATHE 
            };

            var assemblyLine = new AssemblyLine()
            {
                // TODO: generare dati causali del ASSEMBLY_LINE
            };

            var testLine = new TestLine()
            {
                // TODO: generare dati causali del TEST_LINE 
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