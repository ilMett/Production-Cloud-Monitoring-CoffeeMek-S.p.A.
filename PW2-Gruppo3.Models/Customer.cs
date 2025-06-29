using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PW2_Gruppo3.Models;

public class Customer
{
    [Key]
    public Guid Id { get; set; }
    public string Company { get; set; } = string.Empty;
    public string VatCode { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;

    // Proprietà di navigazione: Un cliente può avere molti lotti.
    [JsonIgnore]
    public ICollection<Batch> Batches { get; set; } = [];
}
