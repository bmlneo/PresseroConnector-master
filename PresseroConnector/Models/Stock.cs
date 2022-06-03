using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class Stock
    {
		public string StockId { get; set; }
		public string StockName { get; set; }
		public float PerItemCost { get; set; }
		public int ReorderPoint { get; set; }
	}
}
