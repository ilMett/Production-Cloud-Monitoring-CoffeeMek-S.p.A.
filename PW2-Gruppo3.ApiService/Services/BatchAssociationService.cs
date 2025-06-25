using PW2_Gruppo3.DataGenerator;
using PW2_Gruppo3.DataGenerator.Template;

namespace PW2_Gruppo3.ApiService.Services;

public class BatchAssociationService
{
    public AssemblyLine? ProcessAssemblyLine(Message message)
    {
        if (message?.AssemblyLine == null) return null;

        var assemblyLine = new AssemblyLine
        {
            Machine = message.AssemblyLine.Machine,
            AverageStationTime = message.AssemblyLine.AverageStationTime,
            OperatorsNumber = message.AssemblyLine.OperatorsNumber,
            Faults = message.AssemblyLine.Faults,
            Site = message.AssemblyLine.Site,
            TimeStampLocal = message.AssemblyLine.TimeStampLocal,
            TimeStampUtc = message.AssemblyLine.TimeStampUtc,
            MachineBlockage = message.AssemblyLine.MachineBlockage,
            BlockageCause = message.AssemblyLine.BlockageCause,
            LastMaintenance = message.AssemblyLine.LastMaintenance
        };

        return assemblyLine;
    }

    public Lathe? ProcessLathe(Message message)
    {
        if (message?.Lathe == null) return null;

        var lathe = new Lathe
        {
            Machine = message.Lathe.Machine,
            MachineState = message.Lathe.MachineState,
            RotationSpeed = message.Lathe.RotationSpeed,
            SpindleTemperature = message.Lathe.SpindleTemperature,
            CompletedItems = message.Lathe.CompletedItems,
            Site = message.Lathe.Site,
            TimeStampLocal = message.Lathe.TimeStampLocal,
            TimeStampUtc = message.Lathe.TimeStampUtc,
            MachineBlockage = message.Lathe.MachineBlockage,
            BlockageCause = message.Lathe.BlockageCause,
            LastMaintenance = message.Lathe.LastMaintenance
        };

        return lathe;
    }

    public Milling? ProcessMilling(Message message)
    {
        if (message?.Milling == null) return null;

        var milling = new Milling
        {
            Machine = message.Milling.Machine,
            CycleTime = message.Milling.CycleTime,
            CuttingDepth = message.Milling.CuttingDepth,
            Vibration = message.Milling.Vibration,
            UserAlerts = message.Milling.UserAlerts,
            Site = message.Milling.Site,
            TimeStampLocal = message.Milling.TimeStampLocal,
            TimeStampUtc = message.Milling.TimeStampUtc,
            MachineBlockage = message.Milling.MachineBlockage,
            BlockageCause = message.Milling.BlockageCause,
            LastMaintenance = message.Milling.LastMaintenance
        };

        return milling;
    }

    public TestLine? ProcessTestLine(Message message)
    {
        if (message?.TestLine == null) return null;

        var testLine = new TestLine
        {
            Machine = message.TestLine.Machine,
            TestResult = message.TestLine.TestResult,
            BoilerPressure = message.TestLine.BoilerPressure,
            BoilerTemperature = message.TestLine.BoilerTemperature,
            EnergyConsumption = message.TestLine.EnergyConsumption,
            Site = message.TestLine.Site,
            TimeStampLocal = message.TestLine.TimeStampLocal,
            TimeStampUtc = message.TestLine.TimeStampUtc,
            MachineBlockage = message.TestLine.MachineBlockage,
            BlockageCause = message.TestLine.BlockageCause,
            LastMaintenance = message.TestLine.LastMaintenance
        };

        return testLine;
    }

    public void ProcessTelemetryMessage(Message message)
    {
        // Questo metodo è utile se vuoi chiamarne solo uno
        var assemblyLine = ProcessAssemblyLine(message);
        var lathe = ProcessLathe(message);
        var milling = ProcessMilling(message);
        var testLine = ProcessTestLine(message);

        // Qui puoi eventualmente:  (ma non ci interessano al momento, se non il terzultimo)
        // - salvare i dati su DB
        // - inviare eventi
        // - aggiornare lo stato di un lotto
        // - loggare in modo centralizzato

        Console.WriteLine("Telemetry elaborata con successo.");
    }
}
