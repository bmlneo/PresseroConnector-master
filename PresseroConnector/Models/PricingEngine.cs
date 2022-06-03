using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class PricingEngine
    {
        public string CalculatorId { get; set; }
        public string CalculatorName { get; set; }
        public bool IsActive { get; set; }
        public int Method { get; set; }
        public string SubscriberId { get; set; }
        public IList<UOMList> UOMList { get; set; }
    }
    public class UOMList
    {
        public string ID { get; set; }
        public string UOM { get; set; }
        public double Amount { get; set; }
        public double Weight { get; set; }
        public object WeightUnit { get; set; }
        public object Stock { get; set; }
        public object Ink { get; set; }
        public double Cost { get; set; }
    }
}
