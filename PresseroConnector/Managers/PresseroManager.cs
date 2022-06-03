using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PresseroConnector.Models;

namespace PresseroConnector.Managers
{
    public class PresseroManager
    {

        #region Constants
        private const string URL = "https://admin.sg.v6.pressero.com/api/";

        private const string USERNAME = "awi@epress.com.au";
        private const string PASSWORD = "Epress123$";
        private const string SUBSCRIBERID = "c1af80fb-3e10-4fe9-b1ac-ae660f2a1b96";
        private const string CONSUMERID = "A9EC046E-9F1E-41BA-BAF6-FE06ADD43976";
        #endregion

        private static string token = null;

        public static async Task<IEnumerable<OrderItem>> SearchOrderItems(OrderItemQuery query)
        {
            var orderItems = new List<OrderItem>();
            var pageNumber = 0;
            var pageSize = 1500;
            var totalItems = -1;
            var token = await GetToken();
            // while (totalItems == -1 || totalItems > pageNumber * pageSize)
            do
            {
                var result = await Utility.CreateWebRequest(
                new Uri(URL + "v2/Orders/Search?pageNumber=" + pageNumber + "&pageSize=" + pageSize),
                HttpMethod.Post,
                JsonConvert.SerializeObject(query), token);

                if (result.IsSuccess)
                {
                    var data = Utility.ParseJSON<OrderItemData>((string)result.Data);
                    //Console.WriteLine(data);
                    if (data != null && data.Items != null)
                    {
                        totalItems = data.TotalItems;
                        orderItems.AddRange(data.Items);
                    }
                    pageNumber++;
                    Console.WriteLine(" P#: " + pageNumber + "Total Items: " + totalItems + " Items loaded: " + orderItems.Count);
                }
                else
                {
                    throw new Exception("Error: cannot get all results");
                    //break;
                }
            } while (orderItems.Count < totalItems);

            return orderItems;
        }

        public static async Task<IEnumerable<OrderItem>> SearchOrderItems(string siteUrl, OrderItemQuery query)
        {
            var orderItems = new List<OrderItem>();
            var pageNumber = 0;
            var pageSize = 1500;
            var totalItems = -1;

            while (totalItems == -1 || totalItems > pageNumber * pageSize)
            {

                var result = await Utility.CreateWebRequest(
                new Uri(URL + "site/" + siteUrl + "/orders/?pageNumber=" + pageNumber + "&pageSize=" + pageSize),
                HttpMethod.Post,
                JsonConvert.SerializeObject(query), await GetToken());

                if (result.IsSuccess)
                {
                    var data = JsonConvert.DeserializeObject<OrderItemData>((string)result.Data);
                    if (data != null && data.Items != null)
                    {
                        totalItems = data.TotalItems;
                        pageNumber++;
                        orderItems.AddRange(data.Items);
                    }

                    //return JsonConvert.DeserializeObject<OrderItemData>((string)result.Data).Items;
                }
                else
                {
                    throw new Exception("Error: cannot get all results. " + result.Message);
                    //break;
                }

            }

            //return null;
            return orderItems;
        }
        public static async Task<ShippingMethods> GetShippingMethods()
        {
            var temp = "[{\"Column\": ,\"Value\": ,\"Operator\":}]";
            var shippingResult = new List<string>();
            ShippingMethods shippingData = new ShippingMethods();
            var result = await Utility.CreateWebRequest(
            new Uri(URL + "SubscriberShippingMethods/Search?pageNumber=0&pageSize=200&includeDeleted=false"),
            HttpMethod.Post,
            temp, await GetToken());

            if (result.IsSuccess)
            {
                shippingData = JsonConvert.DeserializeObject<ShippingMethods>((string)result.Data);  
            }
            else
            {
                throw new Exception("Error: cannot get all results. " + result.Message);
            }

            return shippingData;
        }
        public static async Task<ShippingMethodDetails> GetShippingMethodDetails(string id)
        {
            ShippingMethodDetails shippingData = new ShippingMethodDetails();
            var result = await Utility.CreateWebRequest(
            new Uri(URL + "SubscriberShippingMethods/" + id + "/details"),
            HttpMethod.Get,
            null, await GetToken());

            if (result.IsSuccess)
            {
                try
                {
                    shippingData = JsonConvert.DeserializeObject<ShippingMethodDetails>((string)result.Data);
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                throw new Exception("Error: cannot get all results. " + result.Message);
            }

            return shippingData;
        }

        //test
        public static async Task<string> GetAllOrders(string id)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(URL + "site/" + id + "/Orders/?pageNumber=2&pageSize=10"),
                HttpMethod.Get,
                null, await GetToken());

            if (result.IsSuccess)
                return (string)result.Data;

            return null;
        }

        public static async Task<Order> GetOrder(string id)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(URL + "v2/orders/" + id + "/Details"),
                HttpMethod.Get,
                null, await GetToken());

            if (result.IsSuccess)
                return JsonConvert.DeserializeObject<Order>((string)result.Data);

            return null;
        }

        public static async Task<List<ShipmentData>> GetShipment(string orderID, string orderItem)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(URL + "v2/orders/" + orderID + "/Item/" + orderItem + "/Shipments"),
                HttpMethod.Get,
                null, await GetToken());

            if (result.IsSuccess)
            {
                return JsonConvert.DeserializeObject<List<ShipmentData>>((string)result.Data);
            }

            return null;
        }



        public static async Task<string> GetOrderDetails(int orderNumber)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(URL + "v2/orders/" + orderNumber + "/Details"),
                HttpMethod.Get,
                null, await GetToken(), Utility.APPLICATIONJSON);

            if (result.IsSuccess)
                return (string)result.Data;

            return null;
        }
        public static async Task<string> GetOrderItemsDetails(int orderNumber)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(URL + "v2/orders/" + orderNumber + "/items"),
                HttpMethod.Get,
                null, await GetToken(), Utility.APPLICATIONJSON);

            if (result.IsSuccess)
                return (string)result.Data;

            return null;
        }

        public static async Task<string> GetOrderItesDetails(int orderNumber, int itemNumber)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(URL + "v2/orders/" + orderNumber + "/item/" + itemNumber + "/Details"),
                HttpMethod.Get,
                null, await GetToken(), Utility.APPLICATIONJSON);

            if (result.IsSuccess)
                return (string)result.Data;

            return null;
        }

        public static async Task<string> GetOrderItemLegacyDetails(string orderNumber, string itemNumber)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(URL + "v2/orders/" + orderNumber + "/item/" + itemNumber + "/LegacyDetails"),
                HttpMethod.Get,
                null, await GetToken(), Utility.APPLICATIONJSON);

            if (result.IsSuccess)
                return (string)result.Data;

            return null;
        }

        public static async Task<string> GetOrderItemStatusHistory(int orderNumber, int itemNumber)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(URL + "v2/orders/" + orderNumber + "/item/" + itemNumber + "/Status"),
                HttpMethod.Get,
                null, await GetToken(), Utility.APPLICATIONJSON);

            if (result.IsSuccess)
                return (string)result.Data;

            return null;
        }


        public static async Task<string> GetOrderItemPaymentHistory(int orderNumber, int itemNumber)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(URL + "v2/orders/" + orderNumber + "/Payments"),
                HttpMethod.Get,
                null, await GetToken(), Utility.APPLICATIONJSON);

            if (result.IsSuccess)
                return (string)result.Data;

            return null;
        }

        public static async Task<SiteInfo> GetSiteInfo(string id)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(URL + "SiteInfo/" + id),
                HttpMethod.Get,
                null, await GetToken(), Utility.APPLICATIONJSON);

            if (result.IsSuccess)
                return JsonConvert.DeserializeObject<SiteInfo>((string)result.Data);

            return null;
        }

        public static async Task<IEnumerable<ProductInfo>> SearchProducts(string siteUrl)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(string.Format(URL + "site/{0}/products/?pageNumber=0&pageSize=999999&includeDeleted=true", siteUrl)),
                HttpMethod.Get, null, await GetToken());

            if (result.IsSuccess)
            {
                return JsonConvert.DeserializeObject<ProductData>((string)result.Data).Items;
            }

            return null;
        }

        public static async Task<IEnumerable<SubscriberAsset>> SearchSubscriberAssets(string siteUrl)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(string.Format("{0}site/{1}/SubscriberAssets?pageNumber=0&pageSize=999999", URL, siteUrl)),
                HttpMethod.Post, null, await GetToken());

            if (result.IsSuccess)
            {
                return JsonConvert.DeserializeObject<SubscriberAssetData>((string)result.Data).Items;
            }

            return null;
        }

        public static async Task<IEnumerable<Product>> GetProductDetails(string siteUrl)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(string.Format(URL + "site/{0}/products/?pageNumber=0&pageSize=1000&includeDeleted=false", siteUrl)),
                HttpMethod.Get, null, await GetToken());

            List<Product> products = new List<Product>();
            if (result.IsSuccess)
            {
                var productSearchData = JsonConvert.DeserializeObject<ProductData>((string)result.Data).Items;
                foreach (var product in productSearchData)
                {
                    result = await Utility.CreateWebRequest(
                    new Uri(string.Format(URL + "site/{0}/products/{1}", siteUrl, product.ProductId)),
                    HttpMethod.Get, null, await GetToken());

                    products.Add(JsonConvert.DeserializeObject<Product>((string)result.Data));
                }
            }

            return products;
        }

        public static async Task<dynamic> GetProductInfo(string siteUrl, string productID)
        {
            dynamic response = new ExpandoObject();


            var result = await Utility.CreateWebRequest(
                    new Uri(string.Format(URL + "site/{0}/products/{1}", siteUrl, productID)),
                    HttpMethod.Get, null, await GetToken());

            if (result.IsSuccess)
            {
                response = JsonConvert.DeserializeObject<ProductCalculators>((string)result.Data).ProductGroupCalculators;
            }

            return response;
        }

        public static async Task<dynamic> GetCalculatorInfo(string calID)
        {
            dynamic response = new ExpandoObject();


            var result = await Utility.CreateWebRequest(
                    new Uri(string.Format(URL + "Calculators/"+ calID)),
                    HttpMethod.Get, null, await GetToken());

            if (result.IsSuccess)
            {
                response = JsonConvert.DeserializeObject<PricingEngine>((string)result.Data);
            }

            return response;
        }
        public static async Task<dynamic> GetProductList(string siteUrl)
        {
            dynamic response = new ExpandoObject();


            var result = await Utility.CreateWebRequest(
                    new Uri(string.Format(URL + "site/{0}/products/?pageNumber=0&pageSize=1000&includeDeleted=false", siteUrl)),
                    HttpMethod.Get, null, await GetToken());

            if (result.IsSuccess)
            {
                response = JsonConvert.DeserializeObject<ProductData>((string)result.Data).Items;
            }

            return response;
        }

        public static async Task<IEnumerable<dynamic>> GetProductSpecDetails(string siteUrl)
        {
            string[] customers = { "airmaster.impressiveonline.com.au", "alil.impressiveonline.com.au", "basketballvic.impressiveonline.com.au", "cabrini.impressiveonline.com.au", "city.impressiveonline.com.au", "collinsco.impressiveonline.com.au", "CreatureTechnology.easternpress.sg.v6.pressero.com", "dahlsens.easternpress.sg.v6.pressero.com", "dixonkestles.easternpress.sg.v6.pressero.com ", "eptest.easternpress.sg.v6.pressero.com", "fletchers.impressiveonline.com.au", "fmv.impressiveonline.com.au ", "fpaa.impressiveonline.com.au", "fultonhogan.impressiveonline.com.au", "gadens.impressiveonline.com.au", "gpw.impressiveonline.com.au ", "hallandwilcox.easternpress.sg.v6.pressero.com", "henley.impressiveonline.com.au", "hr.impressiveonline.com.au", "htav.impressiveonline.com.au", "jeffersonford.impressiveonline.com.au", "johnhart.easternpress.sg.v6.pressero.com", "kcl.impressiveonline.com.au ", "kenworth.impressiveonline.com.au ", "laerdal.impressiveonline.com.au", "lexusofblackburn.impressiveonline.com.au", "libertymedical.impressiveonline.com.au", "mb.impressiveonline.com.au", "minifab.easternpress.sg.v6.pressero.com ", "mlc.easternpress.sg.v6.pressero.com", "moorestephens.easternpress.sg.v6.pressero.com", "ncp.impressiveonline.com.au ", "ndy.impressiveonline.com.au ", "pedders.impressiveonline.com.au", "pitcherpartners.impressiveonline.com.au ", "proaus.impressiveonline.com.au", "rgs.easternpress.sg.v6.pressero.com", "roberthalf.impressiveonline.com.au ", "sacredheartmission.impressiveonline.com.au", "sevensteps.impressiveonline.com.au ", "shawpartners.easternpress.sg.v6.pressero.com ", "sigma.easternpress.sg.v6.pressero.com", "smec.easternpress.sg.v6.pressero.com ", "stc.impressiveonline.com.au ", "sustainvic.impressiveonline.com.au ", "tic.impressiveonline.com.au ", "travelrite.easternpress.sg.v6.pressero.com", "urbanedge.impressiveonline.com.au", "usgboral.easternpress.sg.v6.pressero.com", "vacc.impressiveonline.com.au", "wilson.impressiveonline.com.au", "wn.impressiveonline.com.au", "xavier.impressiveonline.com.au", "ymca.impressiveonline.com.au" };

            List<dynamic> response = new List<dynamic>();
            foreach (string cus in customers)
            {
                var result = await Utility.CreateWebRequest(
                    new Uri(string.Format(URL + "site/{0}/products/?pageNumber=0&pageSize=1000&includeDeleted=false", cus)),
                    HttpMethod.Get, null, await GetToken());

                if (result.IsSuccess)
                {
                    var productSearchData = JsonConvert.DeserializeObject<ProductData>((string)result.Data).Items;
                    foreach (var product in productSearchData)
                    {
                        dynamic res = await Utility.CreateWebRequest(
                        new Uri(string.Format(URL + "site/{0}/products/{1}", cus, product.ProductId)),
                        HttpMethod.Get, null, await GetToken());

                        dynamic oneItem = new ExpandoObject();
                        var objectData = JsonConvert.DeserializeObject(res.Data);
                        oneItem.ProductName = objectData.ProductName;
                        oneItem.PublicPartNum = objectData.PublicPartNum;
                        oneItem.CalcShowPricePerPiece = objectData.CalcShowPricePerPiece;

                        try
                        {
                            if (objectData.Attributes != null && objectData.Attributes[0] != null && objectData.Attributes[0].AttributeValue != null)
                                oneItem.BillCode = objectData.Attributes[0].AttributeValue;
                            else
                                oneItem.BillCode = "";
                        }
                        catch (Exception e)
                        {
                            oneItem.BillCode = "";
                        }
                        oneItem.siteName = product.SiteName;
                        response.Add(oneItem);
                    }
                }
            }
            return response;
        }

        public static async Task<IEnumerable<InventoryTransaction>> GetInventoryTransactions(string id)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(URL + ""),
                HttpMethod.Post,
                "{}",
                await GetToken());

            if (result.IsSuccess)
                return JsonConvert.DeserializeObject<InventoryTransactionPagination>((string)result.Data).Items;

            return null;
        }

        public static async Task<string> GetWorkflowsById(string id)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(string.Format("{0}workflows/{1}/Stages", URL, id)),
                HttpMethod.Get,
                null,
                await GetToken());

            if (result.IsSuccess)
                return (string)result.Data;

            return null;
        }

        public static async Task<Product> CreateProduct(string siteUrl, Product product)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(string.Format("{0}site/{1}/products", URL, siteUrl)),
                HttpMethod.Post,
                JsonConvert.SerializeObject(product, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                }), await GetToken());

            if (result.IsSuccess)
                return JsonConvert.DeserializeObject<Product>((string)result.Data);

            return null;
        }

        public static async Task<Inventory> CreateStockInventory(string siteUrl, Inventory inventory)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(string.Format(URL + "site/{0}/StockInventory", siteUrl)),
                HttpMethod.Post,
                JsonConvert.SerializeObject(inventory),
                await GetToken());

            if (result.IsSuccess)
                return JsonConvert.DeserializeObject<Inventory>((string)result.Data);

            return null;
        }

        public static async Task<StockCheck> GetStockLevels(string siteId, string currentItem)
        {
            string partRef = currentItem;
            string postParams = "[{" + $"\"Column\": \"StockName\", \"Value\":\"{partRef}\", \"Operator\": \"contains\"" + "}]";

            StockCheck sc = new StockCheck();

            Uri uri = new Uri($"https://admin.sg.v6.pressero.com/api/site/{siteId}/StockInventory/?pageNumber=0&pageSize=1000");

            Result res = await Utility.CreateWebRequest(
                    uri,
                    HttpMethod.Post,
                    postParams,
                    await GetToken()                    
                    );

            if (res.IsSuccess)
            {

                JObject joResponse = JObject.Parse(res.Data.ToString());
                JArray resArray = (JArray)joResponse["Items"];

                sc.isInventoryItem = resArray.Count > 0 ? true : false;

                foreach (var item in resArray)
                {
                    var stockName = item["StockName"].ToString();
                    if (stockName.Contains(partRef))
                    {
                        sc.stockLevel = (int)item["CurrentLevel"];
                    }
                }
            }

            return sc;
        }

        public static async Task<SubscriberAsset> CreateSubscriberAsset(string siteUrl, SubscriberAsset asset)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(string.Format("{0}site/{1}/SubscriberAssets", URL, siteUrl)),
                HttpMethod.Post, JsonConvert.SerializeObject(asset, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                }), await GetToken());

            if (result.IsSuccess)
                return JsonConvert.DeserializeObject<SubscriberAsset>((string)result.Data);

            return null;
        }

        public static async Task<Result> UpdateStaus(string statusId, int orderNumber, int orderItemNumber,
            string notes)
        {
            var result = await Utility.CreateWebRequest(
                new Uri(string.Format("{0}V2/Orders/{1}/Item/{2}/Status/{3}?notes={4}",
                            URL, orderNumber, orderItemNumber, statusId, notes)),
                HttpMethod.Put,
                null, await GetToken(), Utility.APPLICATIONJSON);

            return result;
        }

        public static async Task<Result> UpdateShipments(dynamic Shipment)
        {


            Result result = await Utility.CreateWebRequest(
                new Uri(string.Format("{0}V2/Orders/Shipments", URL)),
                HttpMethod.Post, JsonConvert.SerializeObject(Shipment), await GetToken());

            
            return result;


        }

        public static async Task<Result> UpdateProduct(string site, Product product)
        {
            return await Utility.CreateWebRequest(
                new Uri(string.Format(URL + "site/{0}/products/{1}", site, product.ProductInfo.ProductId)),
                HttpMethod.Put, JsonConvert.SerializeObject(product, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                }), await GetToken());
        }

        public static async Task<Result> UpdateOrderItem(OrderItem orderItem)
        {
            //update items in order
            return await Utility.CreateWebRequest(
                new Uri(string.Format("{0}/v2/Orders/{1}/Item/{2}", URL, orderItem.OrderNumber, orderItem.OrderItemNumber)),
                HttpMethod.Put,
                JsonConvert.SerializeObject(orderItem),
                await GetToken());
        }
        public static async Task<Result> UpdateOrderItem(string orderNo, string orderItemNo, string apibody)
        {
            //update items in order
            return await Utility.CreateWebRequest(
                new Uri(string.Format("{0}/v2/Orders/{1}/Item/{2}", URL, orderNo, orderItemNo)),
                HttpMethod.Put,
                apibody,
                await GetToken());
        }

        public static async Task<Result> UpdateSubscriberAsset(string site, SubscriberAsset asset)
        {
            return await Utility.CreateWebRequest(
                new Uri(string.Format("{0}site/{1}/SubscriberAssets/{2}", URL, site, asset.AssetId)),
                HttpMethod.Put, JsonConvert.SerializeObject(asset, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                }), await GetToken());
        }

        private static async Task<string> GetToken()
        {
            //if (string.IsNullOrEmpty(token))
            if (true)
            {
                var auth = new
                {
                    UserName = USERNAME,
                    Password = PASSWORD,
                    SubscriberId = SUBSCRIBERID,
                    ConsumerID = CONSUMERID
                };

                var result = await Utility.CreateWebRequest(
                    new Uri(URL + "token"), HttpMethod.Post,
                    JsonConvert.SerializeObject(auth), null);

                if (result.IsSuccess)
                {
                    JObject obj = JObject.Parse((string)result.Data);
                    if (obj != null && 200 == (int)obj["Code"])
                    {
                        token = "token " + (string)obj["Data"];
                        //Console.WriteLine("-----" + token);
                    }
                }
            }
            else
            {
                // Pressero does not provide token expiry date,
                // we have to verify the token if it is still valid.
                var result = await Utility.CreateWebRequest(
                    new Uri(string.Format("{0}v2/Authentication/{1}/Validate", URL, token.Substring(6))),
                    HttpMethod.Get,
                    null,
                    null);

                if (!result.IsSuccess &&
                    (!string.IsNullOrEmpty(result.Message) && result.Message.IndexOf("Unauthorized") == -1))
                {
                    token = null;
                    await GetToken();
                }
            }
            //Console.WriteLine("-----" + token);
            return token;
        }
    }
}