using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PW2_Gruppo3.Models;

public class TestLine : ProductionData
{
    public bool TestResult { get; set; }
    public decimal BoilerPressure { get; set; }
    public decimal BoilerTemperature { get; set; }
    public decimal EnergyConsumption { get; set; }
}
