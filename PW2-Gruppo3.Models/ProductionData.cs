using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace PW2_Gruppo3.Models;

public abstract class ProductionData
{
    public Guid Uuid { get; set; }  
    public Guid ItemUuid { get; set; }
    public bool IsFirst { get; set; } 
    public bool IsLast { get; set; }
    public bool MachineBlockage { get; set; }
    public string? BlockageCause { get; set; } 
    public DateTime LastMaintenance { get; set; } 
    public DateTime TimestampLocal { get; set; }
    public DateTime TimestampUtc { get; set; }

    // Foreign Keys
    public Guid BatchUuid { get; set; }
    public Guid SiteUuid { get; set; }

    // Proprietà di navigazione
    public Batch Batch { get; set; } = null!;
}
