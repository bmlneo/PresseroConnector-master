using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class ProductCalculators
    {
        public IList<ProductGroupCalculator> ProductGroupCalculators { get; set; }

    }
    public class Calculator
    {
        public string CalculatorId { get; set; }
        public string CalculatorName { get; set; }
        public bool IsActive { get; set; }
        public int Method { get; set; }
        public string SubscriberId { get; set; }
    }

    public class ProductGroupCalculator
    {
        public int Priority { get; set; }
        public object CalcConfiguration { get; set; }
        public Calculator Calculator { get; set; }
        public string ProductId { get; set; }
        public string GroupId { get; set; }
        public string CalculatorId { get; set; }
    }

}
