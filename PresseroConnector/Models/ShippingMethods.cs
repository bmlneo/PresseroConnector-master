using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
   

    public class Item
    {
        public string SubscriberShippingMethodId { get; set; }
        public bool IsResidential { get; set; }
        public string MethodName { get; set; }
        public double MinCharge { get; set; }
        public bool IsActive { get; set; }
        public double Markup { get; set; }
        public double Handling { get; set; }
        public double? MaxPkgWt { get; set; }
        public string Carrier { get; set; }
        public string ServiceName { get; set; }
        public string SubscriberId { get; set; }
        public string IntegrationID { get; set; }
    }

    public class ShippingMethods
    {
        public List<Item> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }
}
