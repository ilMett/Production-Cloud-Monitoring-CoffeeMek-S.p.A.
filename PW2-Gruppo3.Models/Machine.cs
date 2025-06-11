using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PW2_Gruppo3.Models;

public class Machine
{
    public int Id { get; set; }
    public string MachineId { get; set; } = string.Empty; 
    public string MachineType { get; set; } = string.Empty; 

    // Foreign Key per la tabella Site
    public int SiteId { get; set; }

    public Site Site { get; set; } = null!;

    // Proprietà di navigazione per i dati di produzione
    public ICollection<Milling> Millings { get; set; } = [];
    public ICollection<Lathe> Lathes { get; set; } = [];
    public ICollection<AssemblyLine> AssemblyLines { get; set; } = [];
    public ICollection<TestLine> TestLines { get; set; } = [];
}
