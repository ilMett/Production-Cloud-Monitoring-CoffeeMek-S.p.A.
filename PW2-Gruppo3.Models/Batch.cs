using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PW2_Gruppo3.Models;

public class Batch
{
    public Guid Uuid { get; set; } 
    public int ItemQuantity { get; set; }

    // Foreign Keys
    public int CustomerUuid { get; set; }
    public int SiteUuid { get; set; }

    // Proprietà di navigazione
    public Customer Customer { get; set; } = null!;
    public Site Site { get; set; } = null!;

    // Proprietà di navigazione per i dati di produzione
    public ICollection<Milling> Millings { get; set; } = [];
    public ICollection<Lathe> Lathes { get; set; } = [];
    public ICollection<AssemblyLine> AssemblyLines { get; set; } = [];
    public ICollection<TestLine> TestLines { get; set; } = [];
}
