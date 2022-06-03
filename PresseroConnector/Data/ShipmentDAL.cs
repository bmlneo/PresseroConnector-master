using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using PresseroConnector.Managers;
using PresseroConnector.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Data
{
    public class ShipmentDAL
    {
        private readonly ILogger _logger;
        private readonly IDistributedCache _distributedCache;
        private const string FPAA_SITE_ID = "f25d8783-f7b7-4e9e-974e-04bda9478d22";

        private OrderDAL context;

        public ShipmentDAL(ILogger logger, IDistributedCache distributedCache)
        {
            _logger = logger;
            _distributedCache = distributedCache;
            context = new OrderDAL(_logger, _distributedCache);
        }

 
        public async Task<Result> CompleteShipment(PresseroStatus ordersShip)
        {
            string TOLLURL = "https://www.mytoll.com/?externalSearchQuery={0}&op=Search&url=";
            string TNTURL = "https://www.tntexpress.com.au/interaction/ASPs/Trackcon_tntau.asp?id=TRACK.ASPX&con={0}";
            string STARTRACKURL = "https://sttrackandtrace.startrack.com.au/?txtConsignmentNumber={0}";
            List<FPAAOrderShipment> fpaaOrders = new List<FPAAOrderShipment>();
            Dictionary<string, dynamic> freightMap = new Dictionary<string, dynamic>();
            Result r = new Result();


            try
            {
                List<OrderItem> shipMake = new List<OrderItem>();

                //fetches order with ordernumber
                var result = await context.GetOrder(ordersShip.OrderItems.ToList()[0].OrderNumber);

                foreach (OrderItem item in ordersShip.OrderItems)
                {
                    for (int odex = 0; odex < result.Count(); odex++)
                    {
                        if (result.ElementAt(odex).OrderItemNumber == item.OrderItemNumber)
                        {
                            shipMake.Add(result.ElementAt(odex));
                        }
                    }
                }
                
                dynamic shipDynamic = new System.Dynamic.ExpandoObject();
                dynamic Adrs = new System.Dynamic.ExpandoObject();
                dynamic AdrsProp = new System.Dynamic.ExpandoObject();
                dynamic Ito = new System.Dynamic.ExpandoObject();
                dynamic shipInfo = new System.Dynamic.ExpandoObject();
                List<dynamic> myItems = new List<dynamic>();

                for (int stuff = 0; stuff < shipMake.Count; stuff++)
                {
                    dynamic oid = new System.Dynamic.ExpandoObject();
                    oid.OrderItemId = shipMake.ElementAt(stuff).OrderItemId;
                    myItems.Add(oid);
                }

                AdrsProp.FirstName = shipMake.ElementAt(0).ShipToFirstName;
                AdrsProp.LastName = shipMake.ElementAt(0).ShipToLastName;
                AdrsProp.Address1 = shipMake.ElementAt(0).ShipToAddress1;
                AdrsProp.StateProvince = shipMake.ElementAt(0).ShiptoStateProvince;
                AdrsProp.Postal = shipMake.ElementAt(0).ShipToPostal;
                AdrsProp.Country = shipMake.ElementAt(0).ShipToCountry;
                shipInfo.Cost = shipMake.ElementAt(0).ShipmentCost;
                shipInfo.Address = AdrsProp;
                shipInfo.ShipDate = DateTime.Now.Date;//(itemValue[0].Split('-')[3].Split('/')[2] + "-" + itemValue[0].Split('-')[3].Split('/')[1] + "-" + itemValue[0].Split('-')[3].Split('/')[0]).ToString();

                var trackingCode = ordersShip.Notes;

                if (trackingCode.ToUpper().StartsWith("AUON") || trackingCode.ToUpper().StartsWith("6980") || trackingCode.ToUpper().StartsWith("MYT") || trackingCode.ToUpper().StartsWith("2401"))
                    shipInfo.Tracking = string.Format(TOLLURL, trackingCode);
                else if (trackingCode.ToUpper().StartsWith("EAS"))
                    shipInfo.Tracking = string.Format(TNTURL, trackingCode);
                else if (trackingCode.ToUpper().StartsWith("2AQ"))
                    shipInfo.Tracking = string.Format(STARTRACKURL, trackingCode);
                else
                    shipInfo.Tracking = "Tracking not available for this order. ("+ trackingCode +")";

                shipInfo.Items = myItems;
                shipDynamic.OrderId = shipMake.ElementAt(0).OrderId;
                shipDynamic.SiteShippingMethodName = shipMake.ElementAt(0).ShippingMethod;
                shipDynamic.ShipmentInfo = shipInfo;
                r = await PresseroManager.UpdateShipments(shipDynamic);
                return r;

            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR PROCESSING! -- " + e.Message);
            }

            return r;
        }
    }
}
