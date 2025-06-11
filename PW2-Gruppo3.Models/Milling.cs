using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PW2_Gruppo3.Models;

public class Milling : ProductionData
{
    public int CompletedItemsQuantity { get; set; }
    public decimal CycleDuration { get; set; } // DONE: modificare e mettere DECIMAL
    public decimal CuttingDepth { get; set; }
    public decimal Vibration { get; set; }
    public int UserAlerts { get; set; }
}
