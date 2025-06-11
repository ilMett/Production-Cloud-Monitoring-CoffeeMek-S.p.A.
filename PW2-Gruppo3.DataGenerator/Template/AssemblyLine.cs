namespace PW2_Gruppo3.DataGenerator.Template;

public class AssemblyLine
{
    public string Machine { get; set; }
    public string AverageStationTime { get; set; }
    public string OperatorsNumber { get; set; }
    public string Faults { get; set; }
    public string Site { get; set; }
    public string TimeStampLocal { get; set; }
    public string TimeStampUtc { get; set; }
    public string MachineBlockage { get; set; }
    public string BlockageCause { get; set; }
    public string LastMaintenance { get; set; }
}