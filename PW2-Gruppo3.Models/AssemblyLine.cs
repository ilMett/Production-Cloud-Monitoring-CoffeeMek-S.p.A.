using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PW2_Gruppo3.Models;

public class AssemblyLine : ProductionData
{
    public decimal AverageStationTime { get; set; } // DONE: modificare in DECIMAL
    public int OperatorsNumber { get; set; }
    public string? Faults { get; set; } // Descrizione dei difetti, può essere nullo
}