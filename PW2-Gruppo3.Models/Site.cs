using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PW2_Gruppo3.Models;

public class Site
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Proprietà di navigazione: In un sito possono essere processati molti lotti.
    [JsonIgnore]
    public ICollection<Batch>? Batches { get; set; } = [];
}
