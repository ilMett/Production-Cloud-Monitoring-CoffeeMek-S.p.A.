using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PW2_Gruppo3.Models;

public class Site
{
    public Guid Uuid { get; set; }
    public string Name { get; set; } = string.Empty;

    // Proprietà di navigazione: Un sito può avere molte macchine.
    public ICollection<Machine> Machines { get; set; } = [];

    // Proprietà di navigazione: In un sito possono essere processati molti lotti.
    public ICollection<Batch> Batches { get; set; } = [];
}
