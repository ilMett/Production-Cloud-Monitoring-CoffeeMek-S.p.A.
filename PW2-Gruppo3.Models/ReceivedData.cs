namespace PW2_Gruppo3.Models;

public class ReceivedData
{
    // differisce da DataGenerator.Message perchè questa classe usa le istanze dei models, da capire se è giusto o meno
    public AssemblyLine? AssemblyLine { get; set; }
    public Lathe? Lathe { get; set; }
    public Milling? Milling { get; set; }
    public TestLine? TestLine { get; set; }
}