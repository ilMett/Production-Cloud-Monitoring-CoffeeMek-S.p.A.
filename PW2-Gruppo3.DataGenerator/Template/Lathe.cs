namespace PW2_Gruppo3.DataGenerator.Template;

public class Lathe
{
    public string Machine { get; set; }
    public string MachineState { get; set; }
    public string RotationSpeed { get; set; }
    public string SpindleTemperature { get; set; }
    public string CompletedItems { get; set; }
    public string Site { get; set; }
    public string TimeStampLocal { get; set; }
    public string TimeStampUtc { get; set; }
    public string MachineBlockage { get; set; }
    public string? BlockageCause { get; set; }
    public string LastMaintenance { get; set; }
}