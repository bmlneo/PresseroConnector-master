using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class ShippingMethodDetails
    {
        public bool AddInsurance { get; set; }
        public string Carrier { get; set; }
        public object FreeShippingValue { get; set; }
        public string ServiceCode { get; set; }
        public double Handling { get; set; }
        public object IntegrationID { get; set; }
        public bool IsActive { get; set; }
        public bool IsResidential { get; set; }
        public double Markup { get; set; }
        public string MethodName { get; set; }
        public double MinCharge { get; set; }
        public object MaxPkgWt { get; set; }
        public double? MaxWeight { get; set; }
        public double? MinWeight { get; set; }
        public string SubscriberShippingMethodId { get; set; }
        public IList<Zone> Zones { get; set; }
        public Attributes Attributes { get; set; }
    }
    public class Zone
    {
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
        public int priority { get; set; }
        public double costPerUnit { get; set; }
    }

    public class Attributes
    {
        public string ShipmentSpecialServices { get; set; }
    }
}
