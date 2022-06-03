using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class Order
    {
		public string ID { get; set; }
		public string OrderId { get; set; }
		public int OrderNumber { get; set; }
		public string PrinterOrderId { get; set; }
		public DateTime OrderDate { get; set; }
		public DateTime ReqShipDate { get; set; }
		public string SiteId { get; set; }

		public int OrderItemSeq { get; set; }

		public IEnumerable<OrderItem> Items { get; set; }
	}
}
