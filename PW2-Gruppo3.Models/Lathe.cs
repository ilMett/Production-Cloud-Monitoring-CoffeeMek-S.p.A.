using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PW2_Gruppo3.Models;

public class Lathe : ProductionData
{
    public string MachineState { get; set; } = string.Empty;
    public int RotationSpeed { get; set; } // in RPM
    public decimal SpindleTemperature { get; set; }
    public int CompletedItems { get; set; }
}
