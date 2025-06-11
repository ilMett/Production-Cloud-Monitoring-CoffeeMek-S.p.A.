using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PW2_Gruppo3.Models;

public class Lathe : ProductionData
{
    public string MachineState { get; set; } = string.Empty;
    public int Rpm { get; set; } 
    public decimal SpindleTemperature { get; set; }
    public int CompletedItemsQuantity { get; set; }
}
