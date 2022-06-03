using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class FPAAOrderShipment
    {
        public FPAAOrderShipment(string jobNumber, string orderNumber, string orderItemNumber, string pONumber, string trackingUrl, string dateCompleted)
        {
            JobNumber = jobNumber;
            OrderNumber = orderNumber;
            OrderItemNumber = orderItemNumber;
            PONumber = pONumber;
            TrackingUrl = trackingUrl;
            DateCompleted = dateCompleted;
        }

        public string GetDataAsString()
        {
            return (JobNumber + "," + PONumber + "," + OrderNumber + "," + TrackingUrl);
        }

        public string JobNumber { get; set; }
        public string OrderNumber { get; set; }
        public string OrderItemNumber { get; set; }
        public string PONumber { get; set; }
        public string TrackingUrl { get; set; }
        public string DateCompleted { get; set; }
    }
}
