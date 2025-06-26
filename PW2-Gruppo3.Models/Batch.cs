using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PW2_Gruppo3.Models;

public class Batch
{
    [Key]
    public Guid Id { get; set; } 
    public int ItemQuantity { get; set; }
    public int ItemProduced { get; set; }
    public Guid CustomerId { get; set; }
    public Guid SiteId { get; set; }
    public bool isCompleted { get; set; }

    // Foreign Keys
    public Customer? Customer { get; set; }
    public Site? Site { get; set; }

    // Proprietà di navigazione per i dati di produzione
    public ICollection<Milling> Millings { get; set; } = [];
    public ICollection<Lathe> Lathes { get; set; } = [];
    public ICollection<AssemblyLine> AssemblyLines { get; set; } = [];
    public ICollection<TestLine> TestLines { get; set; } = [];
}
