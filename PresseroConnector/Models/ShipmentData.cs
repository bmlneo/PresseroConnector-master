using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class ShipmentData
    {
        public string ShipmentId { get; set; }
        public string OrderId { get; set; }
        public string OrderItemId { get; set; }
        public DateTime ShipDate { get; set; }
        public string ShipMethod { get; set; }
        public string ShipTracking { get; set; }
        public double ShipCost { get; set; }
        public object IntegrationID { get; set; }
    }
}
