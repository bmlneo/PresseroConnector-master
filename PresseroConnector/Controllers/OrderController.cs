using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PresseroConnector.Models;
using Newtonsoft.Json;
using PresseroConnector.Managers;
using System.Xml.Linq;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Authorization;
using PresseroConnector.Data;
using System.Text;
using System.Net.Http;

namespace PresseroConnector.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly ILogger _logger;
        private readonly IDistributedCache _distributedCache;
        private const string NotificationSvcURL = "https://notifications.epress.com.au/pressero";//http://localhost:5858/pressero


        private OrderDAL context;
        private ShipmentDAL shipContext;
        private String[] workflowTabs = { "Order Received", "Prepress", "Indigo", "Lanier", "GTO", "Laminating", "Guillotine", "Ready To Pick", "Pick Slip Printed", "BRN Benchmark", "On Back Order" };



        public OrderController(ILogger<OrderController> logger, IDistributedCache distributedCache)
        {
            _logger = logger;
            _distributedCache = distributedCache;

            context = new OrderDAL(_logger, _distributedCache);
            shipContext = new ShipmentDAL(_logger, _distributedCache);
        }

        [HttpGet]
        public async Task<string> Get()
        {
            //return null;
            return await context.GetAll(true);
        }

        [HttpPost, Route("Ordersnewpojo")]
        public async Task<IEnumerable<OrderItem>> GetSFOrders([FromBody] InvoiceQuery query)
        {
            System.Diagnostics.Debug.WriteLine("In Pojo");
            Console.WriteLine("GetSFOrders(), query" + query);
            IEnumerable<OrderItem> orderItems = Enumerable.Empty<OrderItem>();

            if (query.To < query.From)
                return null;// BadRequest();

            var dateNow = DateTime.Now;
            var queryStartDate = dateNow.AddMonths(-3);
            var queryEndDate = dateNow;
            //load all orders other than 'Order Completed', 'Invoiced' and 'Order Cancelled' for the last 3 months.

            //load all orders at 'Order Recieved' for last 15 days
            orderItems = await context.GetOrderItems(queryStartDate, queryEndDate);

            return orderItems;
        }

        [HttpPost, Route("FPAAOrdersnewpojo")]
        public async Task<IEnumerable<OrderItem>> GetFPAAOrders([FromBody] InvoiceQuery query)
        {
            Console.WriteLine("FPAA In Pojo");
            IEnumerable<OrderItem> orderItems = Enumerable.Empty<OrderItem>();

            if (query.To < query.From)
                return null;// BadRequest();

            var dateNow = DateTime.Now;
            var queryStartDate = dateNow.AddMonths(-3);
            var queryEndDate = dateNow;
            //load all orders status 'Order Completed' last 3 months.

            //load all orders at 'Order Recieved' for last 15 days
            orderItems = await context.GetFPAAItems(queryStartDate, queryEndDate);
            //orderItems = await context.GetAll();

            return orderItems;
        }



        [HttpPost, Route("SearchOrderpojo")]
        public async Task<IEnumerable<OrderItem>> SearchSFOrder([FromBody] InvoiceQuery query)
        {
            System.Diagnostics.Debug.WriteLine("In Pojo");

            if (query.To < query.From)
                return null;// BadRequest();

            var dateNow = DateTime.Now;
            var queryStartDate = dateNow.AddMonths(-3);
            var queryEndDate = dateNow;

            var allOrderItems = new Dictionary<(int, int), OrderItem>();


            var filters = new FilterExpression[]
            {
                    new FilterExpression()
                    {
                        Column = "OrderNumber",
                        Value = query.OrderNumber,
                        Operator = "equals"
                    }
            };

            IEnumerable<OrderItem> orderItems;
            try
            {
                orderItems = await context.Get(queryStartDate, queryEndDate, filters);



                //orderItems = orderItems1;//
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
                //return StatusCode(500, ex.Message);
            }

            return orderItems;
            //return await context.Get("dahlsens.easternpress.sg.v6.pressero.com");
        }

        [HttpGet, Route("OrderItemsDetailspojo/{orderNumber:int}")]
        public async Task<dynamic> GetOrderItemsDetails(int orderNumber)
        {
            return await context.GetOrderItemsDetails(orderNumber);
        }

        [HttpGet, Route("{orderNumber:int}")]
        public async Task<IEnumerable<OrderItem>> GetOrder(int orderNumber)
        {
            return await context.GetOrder(orderNumber);
        }

        //[HttpGet, Route("{orderNumber:int}/{itemNumber:int}")]
        //public async Task<OrderItem> GetItem(int orderNumber, int itemNumber)
        //{
        //    var orderItems = await context.GetOrder(orderNumber);
        //    foreach (var item in orderItems)
        //    {
        //        if (item.OrderNumber == orderNumber && item.OrderItemNumber == itemNumber)
        //        {
        //            return item;
        //        }
        //    }

        //    return null;
        //}

        // Stock check
        [HttpGet]
        [Route("")]
        [Route("{siteId}/{productCode}/stockLevelCheck")]
        public async Task<int> StockLevels(string siteID, string productCode)
        {
            var data = await PresseroManager.GetStockLevels(siteID, productCode);

            return data.stockLevel;
        }

        // order status history
        [HttpGet, Route("{orderNumber:int}/{itemNumber:int}/statusHistory")]
        public async Task<IActionResult> GetItemStatusHistory(int orderNumber, int itemNumber)
        {
            var data = await PresseroManager.GetOrderItemStatusHistory(orderNumber, itemNumber);
            if (string.IsNullOrEmpty(data))
                return NotFound();

            return Content(data, "application/json");
        }




        //// order payment history

        [HttpGet]
        [Route("{orderNumber:int}/{itemNumber:int}/paymentHistory")]
        public async Task<IActionResult> GetItemPaymentHistory(int orderNumber, int itemNumber)
        {
            var data = await PresseroManager.GetOrderItemPaymentHistory(orderNumber, itemNumber);
            if (string.IsNullOrEmpty(data))
                return NotFound();

            return Content(data, "application/json");
        }

        [HttpGet, Route("SiteInfo/{SiteID}")]
        public async Task<IActionResult> GetSiteInfo(string SiteID)
        {
            var data = await PresseroManager.GetSiteInfo(SiteID);
            return Ok(data);
        }

        [HttpGet, Route("GetShipment/{orderID}/{orderItem}")]
        public async Task<IActionResult> GetSiteInfo(string orderID, string orderItem)
        {
            var data = await PresseroManager.GetShipment(orderID, orderItem);
            return Ok(data);
        }

        //

        [HttpGet, Route("{orderNumber}/{itemNumber}/Legacydetails")]
        public async Task<IActionResult> GetOrderItemLegacyDetails(string orderNumber, string itemNumber)
        {
            var data = await PresseroManager.GetOrderItemLegacyDetails(orderNumber, itemNumber);
            if (string.IsNullOrEmpty(data))
                return NotFound();

            return Content(data, "application/json");
        }

        [HttpGet, Route("{orderNumber:int}/{itemNumber:int}/details")]
        public async Task<IActionResult> GetOrderItemDetails(int orderNumber, int itemNumber)
        {
            var data = await PresseroManager.GetOrderItesDetails(orderNumber, itemNumber);
            if (string.IsNullOrEmpty(data))
                return NotFound();

            return Content(data, "application/json");
        }

        [HttpGet, Route("{siteName}/productDetails")]
        public async Task<IActionResult> GetSiteProductDetails(string siteName)
        {
            var data = await PresseroManager.GetProductSpecDetails(siteName);


            return Ok(data);
        }

        [HttpGet, Route("{siteName}/productList")]
        public async Task<IActionResult> GetSiteProducts(string siteName)
        {
            var data = await PresseroManager.GetProductList(siteName);



            return Ok(data);
        }
        [HttpGet, Route("{siteName}/{productID}")]
        public async Task<IActionResult> GetProductDetails(string siteName, string productID)
        {
            var data = await PresseroManager.GetProductInfo(siteName, productID);



            return Ok(data);
        }

        [HttpGet, Route("Calculators/{calID}")]
        public async Task<IActionResult> GetCalculatorbyID(string calID)
        {
            var data = await PresseroManager.GetCalculatorInfo(calID);



            return Ok(data);
        }

        [HttpGet, Route("Upload")]
        public async Task Upload()
        {
            //await context.FixAssetNames("libertymedical.impressiveonline.com.au");
            /*
			await context.UploadAssets("libertymedical.impressiveonline.com.au",
				@"\\EPRESSSTG02\AraxiVolume_EPRESSSTG02_J\Jobs\Storefront Files\L\Liberty Medical\To Storefront\INVENTORY\PDFs\Downloadable Assets");
			*/
            await Task.FromResult(0);
        }

        [HttpGet, Route("Refresh")]
        public async Task Refresh()
        {
            await context.GetAll(true);
        }

        [HttpPut, Route("UpdateOrderItem")]
        public async Task<IActionResult> UpdateOrderItem([FromBody] OrderItem query)
        {

            string apiBody = "{\"MisId\": \"" + query.OrderItemMISID + "\"}";
            var result = await PresseroManager.UpdateOrderItem(query.OrderNumber.ToString(), query.OrderItemNumber.ToString(), apiBody);

            if (!result.IsSuccess)
                return BadRequest();
            else
                return Ok(result);
        }

        [HttpPost, AllowAnonymous]
        public async Task Create([FromBody] EventData eventData)
        {
            _logger.LogInformation($"Create(): New event, Order: {eventData.Data.OrderNumber}, TypeNumber: {eventData.Type} TypeName: {eventData.TypeName}, SiteId: {eventData.SiteId}, SubscriberId: {eventData.SubscriberId}\n{eventData.Data}");

            Console.WriteLine($"Create(): New event, Order: {eventData.Data.OrderNumber}, TypeNumber: {eventData.Type} TypeName: {eventData.TypeName}, SiteId: {eventData.SiteId}, SubscriberId: {eventData.SubscriberId}\n{eventData.Data}");
            //return; //PULL THE PLUG NM
            var orderItems = await context.GetOrder(eventData.Data.OrderNumber, true);
            if (!orderItems.Any())
                return;

            // Check  for order items that are yet to be approved or may be rejected.
            var pendingOrderItems = GetPendingOrderItems(orderItems);
            // Only process the order once there are no pending items.
            if (!pendingOrderItems.Any())
            {
                // Order Placed
                if (eventData.Type == 10)
                {
                    // Job creation disabled, requires Prism upgrade// await context.CreateJob(order, item); // Send order data to Nichemark.
                    await SendOrderToSupplier(orderItems);
                }
                // Order item approved
                else if (eventData.Type == 7)
                {
                    var orderItem = orderItems.Where(i => i.OrderItemNumber == eventData.Data.OrderItemSeq).First();

                    // Job creation disabled, requires Prism upgrade. // await context.CreateJob(order, item); // Send order data to Nichemark.
                    await SendOrderToSupplier(orderItems);
                }
                // Order item denied/rejected
                else if (eventData.Type == 9)
                {
                    var orderItem = orderItems.Where(i => i.OrderItemNumber == eventData.Data.OrderItemSeq).First();

                    await SendOrderToSupplier(orderItems);
                }
            }

            // Notify our notification service to update any clients listening for updates.
            var result = await Utility.CreateWebRequest(
                new Uri(NotificationSvcURL),
                HttpMethod.Post,
                "[{OrderNumber:" + eventData.Data.OrderNumber + "}]",
                null);
        }

        [HttpPost, AllowAnonymous, Route("ManualCreate")]
        public async Task ManualCreate([FromBody] IEnumerable<OrderItem> orderItems)
        {
            // Check  for order items that are yet to be approved or may be rejected.
            var pendingOrderItems = GetPendingOrderItems(orderItems);
            var orderNumber = orderItems.First().OrderNumber;
            // Only process the order once there are no pending items.
            if (!pendingOrderItems.Any())
            {
                // Order Placed
                await SendOrderToSupplier(orderItems);
            }

            // Notify our notification service to update any clients listening for updates.
            var result = await Utility.CreateWebRequest(
                new Uri(NotificationSvcURL),
                HttpMethod.Post,
                "[{OrderNumber:" + orderNumber + "}]",
                null);
        }

        [HttpPost, Route("Status")]
        public async Task<IActionResult> Status([FromBody] PresseroStatus status)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            foreach (var item in status.OrderItems)
            {
                var result = await PresseroManager.UpdateStaus(status.ID, item.OrderNumber,
                    item.OrderItemNumber, status.Notes);

                if (!result.IsSuccess)
                    return BadRequest();
            }
            if (status.Notes != null && status.Notes != "No Order Match, Updated from Pressero Connector")
            {
                if (status.Notes.Trim() != "")
                {
                    Result r = await shipContext.CompleteShipment(status);
                    if (r.IsSuccess == false)
                    {
                        await Utility.NotifyAdmin("Presserro Connector Add Shipment Error", r.Message);
                    }
                }
            }

            await context.GetOrder(status.OrderItems.First().OrderNumber, true);

            var queryStartDate = DateTime.Today.AddDays(1).AddMonths(-3);
            var queryEndDate = DateTime.Today.AddDays(1);
            context.GetOrderItems(queryStartDate, queryEndDate, true);

            return Ok();
        }

        [HttpPost, Route("BackOrderStatus")] 
        public async Task<IActionResult> BackOrderStatus([FromBody]PresseroStatus status)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            foreach (var item in status.OrderItems)
            {
                var result = await PresseroManager.UpdateStaus(status.ID, item.OrderNumber,
                    item.OrderItemNumber, status.Notes);

                if (!result.IsSuccess)
                    return BadRequest();
            }

            return Ok();
        }

        [HttpGet, Route("UpdateCache")]
        public async Task<IActionResult> UpdateCache()
        {
            var queryStartDate = DateTime.Today.AddDays(1).AddMonths(-3);
            var queryEndDate = DateTime.Today.AddDays(1);
            await context.GetOrderItems(queryStartDate, queryEndDate, true);

            return Ok();
        }

        [HttpPost, Route("Shipments")]
        public async Task<IActionResult> Shipments([FromBody] dynamic Shipments)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var result = await PresseroManager.UpdateShipments(Shipments);

            if (!result.IsSuccess)
                return BadRequest();


            else

                return Ok();
        }

        /// <summary>
        /// Returns order items that are yet to approved or rejected.
        /// </summary>
        /// <returns>The pending order items.</returns>
        /// <param name="orderItems">Order items.</param>
        List<OrderItem> GetPendingOrderItems(IEnumerable<OrderItem> orderItems)
        {
            List<OrderItem> pendingOrderItems = new List<OrderItem>();
            foreach (var item in orderItems)
            {
                if (!item.IsApproved && !item.IsDenied)
                {
                    pendingOrderItems.Add(item);
                }
            }

            return pendingOrderItems;
        }

        async Task SendOrderToSupplier(IEnumerable<OrderItem> orderItems)
        {
            var siteId = orderItems.First().SiteId;
            _logger.LogInformation($"Top of SendOrderToSupplier() - siteID: {siteId}");

            switch (siteId)
            {
                case Constants.LIBERTY_MEDICAL_SITE_ID:
                    Console.WriteLine($"SendOrderToSupplier() SiteName: {orderItems.First().SiteName}");

                    await FTPManager.UploadOrderItemToNM(_logger, orderItems);

                    if (orderItems.Any())
                    {
                        var statusId = "37ca89bd-60fb-4a07-a0bd-65983661fcc8"; // BRN Benchmark ID (Storefront)
                        var statusNote = "BRN, Updated from Pressero Connector";
                        var res = await SetConditionalStatusAsync(orderItems, statusId, statusNote);
                    }
                    break;
                case Constants.PEDDERS_SITE_ID:
                case Constants.HTAV_SITE_ID:
                case Constants.HIGGINS_SITE_ID:
		        case Constants.SMEC_SITE_ID: //19-05-22
		        case Constants.DAHLSENS_SITE_ID: 
		        case Constants.COLLINS_SITE_ID: 
		        case Constants.FULTON_SITE_ID: 
                case Constants.CATHSUPER_SITE_ID: //3-06-22
                case Constants.NDY_SITE_ID:
                case Constants.RHALF_SITE_ID:
                case Constants.GADENS_SITE_ID:
                case Constants.PITCHER_SITE_ID: //14-06-22



                    // IMPORTANT - THE FOLLOWING RULES APPLY TO PEDDERS AND HTAV
                    // For Pedders and HTAV, send only inventory order items
                    // to external supplier Nichemark.
                    List<OrderItem> inventoryItems = new List<OrderItem>();

                    foreach (var item in orderItems)
                    {
                        //if status is order recived then and billing code is not INV-PP-EXT-PED
                        if (OrderItemIsInventoryItem(item) && item.ProductAttributeValue1 != "INV-PP-EXT-PED")
                        {
                            inventoryItems.Add(item);
                        }
                    }

                    if (inventoryItems.Any())
                    {
                        await FTPManager.UploadOrderItemToNM(_logger, inventoryItems);

                        var statusId = "37ca89bd-60fb-4a07-a0bd-65983661fcc8"; // BRN Benchmark ID (Storefront)
                        var statusNote = "BRN, Updated from Pressero Connector";
                        var res = await SetConditionalStatusAsync(inventoryItems, statusId, statusNote);
                    }

                    break;
                case Constants.CONVATEC_SITE_ID:

                    // For convatec, send only inventory order items
                    // to external supplier Nichemark.
                    // send orders which are placed with 'convatec urgent shipping' directly, rest orders will be batch at month end
                    List<OrderItem> inventoryBillOnDemandItems1 = new List<OrderItem>();

                    foreach (var item in orderItems)
                    {
                        if (OrderItemIsInventoryItem(item) && item.ProductAttributeValue1 != "INV-PP-EXT-PED")
                        {
                            inventoryBillOnDemandItems1.Add(item);
                        }
                    }
                    if (inventoryBillOnDemandItems1.Any() && orderItems.First().ShippingMethod.ToLower().Equals("convatec urgent shipping"))//Constants.CONVATEC_ADMIN_EMAILS.Contains(orderItems.First().OrderedByEmail.ToLower().ToString()))
                    {
                        await FTPManager.UploadOrderItemToNM(_logger, inventoryBillOnDemandItems1);

                        var statusId = "37ca89bd-60fb-4a07-a0bd-65983661fcc8"; // BRN Benchmark ID (Storefront)
                        var statusNote = "BRN, Updated from Pressero Connector";
                        var res = await SetConditionalStatusAsync(inventoryBillOnDemandItems1, statusId, statusNote);
                    }

                    break;
                //case Constants.FPAA_SITE_ID:
                /*List<OrderItem> orderItemsFPAA = new List<OrderItem>();
                foreach (var item in orderItems)
                {
                    if (OrderItemFPAA(item))
                    {
                        if (item.IsPaid)
                        {
                            orderItemsFPAA.Add(item);
                        }
                    }
                }
                if (orderItemsFPAA.Any())
                {
                    await FTPManager.UploadOrderItemToNM(_logger, orderItemsFPAA);
                }

                if (orderItemsFPAA.Any())
                {
                    var statusId = "37ca89bd-60fb-4a07-a0bd-65983661fcc8"; // BRN Benchmark ID (Storefront)
                    var statusNote = "BRN, Updated from Pressero Connector";
                    var res = await SetConditionalStatusAsync(orderItemsFPAA, statusId, statusNote);
                }

                List<OrderItem> orderItemsBillMatchFPAA = new List<OrderItem>();

                foreach (var item in orderItems)
                {
                    if (OrderItemGenericCheck(item,"INV-BAO"))
                    {
                        orderItemsBillMatchFPAA.Add(item);
                    }
                }
                if (orderItemsBillMatchFPAA.Any())
                {
                    var statusId = "4d9ea30e-bbd2-4b8f-b440-363e76ce25eb"; // Dispatch ID (Storefront)
                    var statusNote = "FPAA, Dispatch, Updated from Pressero Connector";
                    var res = await SetConditionalStatusAsync(orderItemsBillMatchFPAA, statusId, statusNote);
                }
                */
                //break;
                default:
                    {
                        if (siteId != Constants.LIBERTY_MEDICAL_SITE_ID && siteId != Constants.PEDDERS_SITE_ID)
                        {

                            List<OrderItem> OrderItemsNoMatch = new List<OrderItem>();
                            foreach (var item in orderItems)
                            {
                                if (OrderItemGenericCheck(item, "INV-PP") || OrderItemGenericCheck(item, "INV-"))
                                {
                                    OrderItemsNoMatch.Add(item);
                                }
                            }
                            if (OrderItemsNoMatch.Any())
                            {
                                var statusId = "4d9ea30e-bbd2-4b8f-b440-363e76ce25eb"; // Dispatch ID (Storefront)
                                var statusNote = "No Order Match, Updated from Pressero Connector";
                                var res = await SetConditionalStatusAsync(OrderItemsNoMatch, statusId, statusNote);
                            }
                        }
                    }
                    break;
            }
        }
        public string CreateReferenceNo(OrderItem oItem)
        {
            string shipAdd = oItem.ShipToAddress1.Replace(",", "");
            shipAdd = shipAdd.Trim();
            return "EP Batch " + DateTime.Now.Date.Day + DateTime.Today.ToString("MMM").ToUpper() + " " + shipAdd.Substring(0, 1).ToUpper() +
                shipAdd.Substring(shipAdd.Length - 1, 1).ToUpper() + oItem.ShipToCity.Substring(0, 1).ToUpper() +
                oItem.ShipToPostal.Substring(0, 1).ToUpper() + "-" + CreateRandomString(4);

        }

        public static int RandomRange(int min, int max)
        {
            Random r = new Random();

            return r.Next(max - min) + min;
        }

        public static string CreateRandomString(int len)
        {
            var tmpString = "";
            string alpha = "abcdefghijklmnopqrstuvwxyz0123456789";

            for (int i = 0; i < len; i++)
            {
                tmpString += alpha[RandomRange(0, alpha.Length)];
            }

            return tmpString.ToUpper();
        }

        public int GetRandomRange(int min, int max)
        {
            Random ran = new Random();

            return ran.Next(max - min) + min;
        }

        static bool OrderItemIsInventoryItem(OrderItem item)
        {
            // Future proofing this, I could've got away with just checking product 
            // attribute values at the time of implementing this.
            var index = Array.FindIndex(
                item.GetAttributeNamesAsArray(),
                x => x.Equals("Billing Category",
                              StringComparison.InvariantCultureIgnoreCase));

            var billingCat = item.GetAttributeValuesAsArray()[index];

            if (index >= 0 &&
                billingCat.IndexOf("INV-", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                return true;
            }

            return false;
        }

        static bool OrderItemFPAA(OrderItem item)
        {
            // Future proofing this, I could've got away with just checking product 
            // attribute values at the time of implementing this.
            var index = Array.FindIndex(
                item.GetAttributeNamesAsArray(),
                x => x.Equals("Billing Category",
                              StringComparison.InvariantCultureIgnoreCase));

            var billingCat = item.GetAttributeValuesAsArray()[index];

            if (index >= 0 &&
                billingCat.IndexOf("INV-PP_EXT", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                return true;
            }

            return false;
        }

        static bool OrderItemGenericCheck(OrderItem item, string billMatch)
        {
            if (item == null) return false;
            // Future proofing this, I could've got away with just checking product 
            // attribute values at the time of implementing this.
            var index = Array.FindIndex(
                item.GetAttributeNamesAsArray(),
                x => x.Equals("Billing Category",
                              StringComparison.InvariantCultureIgnoreCase));

            var billingCat = item.GetAttributeValuesAsArray()[index];

            if (index >= 0 &&
                billingCat.IndexOf(billMatch, StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                return true;
            }

            return false;
        }

        private async Task<bool> SetConditionalStatusAsync(IEnumerable<OrderItem> OrderItemsNoMatch, string statusId, string statusNotes)
        {

            var itemsGroupedByShipTo = OrderItemsNoMatch.GroupBy(i => i.ShipToAddress1 + i.ShipToAddress2 + i.ShipToAddress3 + i.ShipToCity + i.ShipToPostal);

            PresseroStatus brnStatus = new PresseroStatus();
            List<OrderItem> brnItemRec = new List<OrderItem>();
            brnStatus.ID = statusId;
            brnStatus.Notes = statusNotes;

            foreach (var group in itemsGroupedByShipTo)
            {
                foreach (var item in group)
                {
                    if (!item.IsApproved || item.IsDenied)
                        continue;

                    brnItemRec.Add(item);

                }
            }

            if (brnItemRec.Any())
            {
                brnStatus.OrderItems = brnItemRec;
                var res = await Status(brnStatus);
                if (res.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
                {


                    return true;
                }

                else

                    return false;

            }

            return true;

        }
    }
}
