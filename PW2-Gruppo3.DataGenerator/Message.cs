using PW2_Gruppo3.DataGenerator.Template;

namespace PW2_Gruppo3.DataGenerator;

public class Message
{
    public AssemblyLine AssemblyLine { get; set; }
    public Lathe Lathe { get; set; }
    public Milling Milling { get; set; }
    public TestLine TestLine { get; set; }
}