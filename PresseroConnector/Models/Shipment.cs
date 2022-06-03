using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class Shipment
    {
		public DateTime ShipDate { get; set; }
		public string Tracking { get; set; }
		public float Cost { get; set; }

		public Address Address { get; set; }

		public IEnumerable<OrderItem> Items { get; set; }
    }
}
