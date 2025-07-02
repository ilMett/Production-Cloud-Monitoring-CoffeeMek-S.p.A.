using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PW2_Gruppo3.Models;

public abstract class ProductionData
{
    [Key]
    public Guid Id { get; set; }  
    public Guid MachineryId { get; set; }
    public bool IsFirst { get; set; } 
    public bool IsLast { get; set; }
    public bool MachineBlockage { get; set; }
    public string? BlockageCause { get; set; } 
    public DateTime LastMaintenance { get; set; } 
    public DateTime TimestampLocal { get; set; }
    public DateTime TimestampUtc { get; set; }

    // Foreign Keys
    public Guid BatchId { get; set; }
    public Guid SiteId { get; set; }

    // Proprietà di navigazione
    [JsonIgnore]
    public Batch Batch { get; set; } = null!;
}
