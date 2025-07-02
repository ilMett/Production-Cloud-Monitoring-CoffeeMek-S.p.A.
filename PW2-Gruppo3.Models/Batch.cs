using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace PW2_Gruppo3.Models;

public class Batch
{
    [Key]
    public Guid Id { get; set; } 
    public int ItemQuantity { get; set; }
    public int? ItemProduced { get; set; }
    public Guid CustomerId { get; set; }
    public Guid SiteId { get; set; }
    public bool isCompleted { get; set; } = false;

    // Foreign Keys
    [JsonIgnore]
    public Customer? Customer { get; set; }

    [JsonIgnore]
    public Site? Site { get; set; }

    // Proprietà di navigazione per i dati di produzione
    [JsonIgnore]
    public ICollection<Milling>? Millings { get; set; } = [];

    [JsonIgnore]
    public ICollection<Lathe>? Lathes { get; set; } = [];

    [JsonIgnore]
    public ICollection<AssemblyLine>? AssemblyLines { get; set; } = [];

    [JsonIgnore]
    public ICollection<TestLine>? TestLines { get; set; } = [];
}
