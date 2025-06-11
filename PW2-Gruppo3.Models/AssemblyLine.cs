using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PW2_Gruppo3.Models;

public class AssemblyLine : ProductionData
{
    public int AverageStationTime { get; set; } // TODO: modificare in DECIMAL
    public int OperatorsNumber { get; set; }
    public string? Faults { get; set; } // Descrizione dei difetti, può essere nullo
}