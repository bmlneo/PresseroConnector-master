using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class OrderShipmentData
    {
		public string OrderId { get; set; }
		public string SiteShippingMethodName { get; set; }

		public Shipment ShipmentInfo { get; set; }
	}
}
