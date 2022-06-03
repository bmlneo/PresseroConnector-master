using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PresseroConnector.Managers;
using PresseroConnector.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PresseroConnector.Data
{
    public class OrderDAL
    {

        private readonly ILogger _logger;
        private readonly IDistributedCache _distributedCache;
        private String[] workflowTabs = { "Order Received", "Prepress", "Indigo", "Lanier", "GTO", "Laminating", "Guillotine", "Ready To Pick", "Pick Slip Printed", "BRN Benchmark", "On Back Order" };

#if DEBUG
        private const string CACHE_KEY_PRESSERO_ORDERS = "PRESSERO_ORDERS_TEST";
#else
		private const string CACHE_KEY_PRESSERO_ORDERS = "PRESSERO_ORDERS";
#endif

        private const string CACHE_KEY_PRESSERO_ORDERS_DEBUG = "PRESSERO_ORDERS_TEST_DEBUG";
        public OrderDAL(ILogger logger, IDistributedCache distributedCache)
        {
            _logger = logger;
            _distributedCache = distributedCache;

            var queryStartDate = DateTime.Today.AddDays(1).AddMonths(-3);
            var queryEndDate = DateTime.Today.AddDays(1);
        }

        public async Task<string> GetAll(bool refetch = false)
        {
            var query = new OrderItemQuery()
            {
                StartDate = DateTime.Now.AddMonths(-3),
                EndDate = DateTime.Now,
                SortDescriptor = new SortDescriptor()
                {
                    Member = "OrderNumber",
                    Direction = "Ascending"
                }
            };

            string orderData = null;
            if (!refetch)
            {
                orderData = await _distributedCache.GetStringAsync(CACHE_KEY_PRESSERO_ORDERS);
            }

            // If we have no order item data or want to refetch.
            if (orderData == null || orderData == "null")
            {
                // Fetch all order items
                var orders = await PresseroManager.SearchOrderItems(query);

                orderData = JsonConvert.SerializeObject(orders);

                // Save to our cache
                await _distributedCache.SetStringAsync(
                    CACHE_KEY_PRESSERO_ORDERS, orderData);
            }

            return orderData;
        }

        public async Task<IEnumerable<OrderItem>> Get(DateTime from, DateTime to,
            FilterExpression[] filters = null)
        {
            var query = new OrderItemQuery()
            {
                StartDate = from,
                EndDate = to,
                SortDescriptor = new SortDescriptor()
                {
                    Member = "OrderNumber",
                    Direction = "Ascending"
                },
                Filters = filters
            };

            return await PresseroManager.SearchOrderItems(query);
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItems(DateTime from, DateTime to, bool refetch = false)
        {
            IEnumerable<OrderItem> orderItems = Enumerable.Empty<OrderItem>();
            try
            {
                var orderData = await _distributedCache.GetStringAsync(CACHE_KEY_PRESSERO_ORDERS_DEBUG);
                //Console.WriteLine(orderData);
                orderItems = JsonConvert.DeserializeObject<IEnumerable<OrderItem>>(orderData);
            }
            catch (Exception e)
            {
                refetch = true;
            }
            if (orderItems.Any() && !refetch)
                return orderItems;
            else
            {

                await _distributedCache.RemoveAsync(CACHE_KEY_PRESSERO_ORDERS_DEBUG);
                orderItems = Enumerable.Empty<OrderItem>();

                foreach (string s in workflowTabs)
                {
                    var filters = new FilterExpression[]
                    {
                    new FilterExpression()
                    {
                        Column = "WorkflowStagePrinter",
                        Value = s,
                        Operator = "equals"
                    }
                    };

                    try
                    {
                        IEnumerable<OrderItem> orderItem;
                        orderItem = await this.Get(from, to, filters);
                        orderItems = orderItems.Concat(orderItem);
                        //Console.WriteLine("--");

                        //orderItems = orderItems1;//
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return orderItems;
                        //return StatusCode(500, ex.Message);
                    }
                }

                var filtersOrderCompleted = new FilterExpression[]
                    {
                    new FilterExpression()
                    {
                        Column = "WorkflowStagePrinter",
                        Value = "Order Completed",
                        Operator = "equals"
                    }
                    };

                var queryStartDateCompleted = DateTime.Today.AddDays(-2);
                var queryEndDateCompleted = DateTime.Today.AddDays(1);
                IEnumerable<OrderItem> orderItemCompleted = await this.Get(queryStartDateCompleted, queryEndDateCompleted, filtersOrderCompleted);
                orderItems = orderItems.Concat(orderItemCompleted);

                // Save to our cache
                await _distributedCache.SetStringAsync(
                    CACHE_KEY_PRESSERO_ORDERS_DEBUG, JsonConvert.SerializeObject(orderItems));
            }
            return orderItems;
        }

        public async Task<IEnumerable<OrderItem>> GetFPAAItems(DateTime from, DateTime to)
        {
            IEnumerable<OrderItem> orderItems = Enumerable.Empty<OrderItem>();
            orderItems = Enumerable.Empty<OrderItem>();

            //var filtersOrderCompleted = new FilterExpression[]
            //    {
            //        new FilterExpression()
            //        {
            //            Column = "WorkflowStagePrinter",
            //            Value = "Order Completed",
            //            Operator = "equals"
            //        }
            //    };
            var today = DateTime.Now;
            from = today.AddDays(-24);
            to = DateTime.Now;

            IEnumerable<OrderItem> orderItemCompleted = await this.Get("fpaa.impressiveonline.com.au", from, to);
            orderItems = orderItems.Concat(orderItemCompleted);

            // Save to our cache
            //await _distributedCache.SetStringAsync(
            //    CACHE_KEY_PRESSERO_ORDERS_DEBUG, JsonConvert.SerializeObject(orderItems));
            return orderItems;
        }

        public async Task<IEnumerable<OrderItem>> Get(string siteUrl, DateTime from, DateTime to,
            FilterExpression[] filters = null)
        {
            var query = new OrderItemQuery()
            {
                StartDate = from,
                EndDate = to,
                SortDescriptor = new SortDescriptor()
                {
                    Member = "OrderNumber",
                    Direction = "Ascending"
                },
                Filters = filters
            };

            return await PresseroManager.SearchOrderItems(siteUrl, query);
        }

        public async Task<dynamic> GetOrderItemsDetails(int orderNumber)
        {
            var orderItems = await PresseroManager.GetOrderItemsDetails(orderNumber);
            var orderDetails = await PresseroManager.GetOrderDetails(orderNumber);

            var returnItems = JsonConvert.DeserializeObject(orderItems);
            var returnDetails = JsonConvert.DeserializeObject(orderDetails);

            var resultJson = JsonConvert.SerializeObject(new { orderDetails = returnDetails, orderItems = returnItems });
            return resultJson;
        }

        public async Task<IEnumerable<OrderItem>> GetOrder(int orderNumber, bool refetch = false)
        {
            var query = new OrderItemQuery()
            {
                StartDate = DateTime.Now.AddMonths(-3),
                EndDate = DateTime.Now,
                SortDescriptor = new SortDescriptor()
                {
                    Member = "OrderNumber",
                    Direction = "Ascending"
                },
                Filters = new FilterExpression[]
                {
                    new FilterExpression("OrderNumber", orderNumber.ToString(),"equals")
                }
            };

            string orderData = await _distributedCache.GetStringAsync(CACHE_KEY_PRESSERO_ORDERS);
            if (orderData == null || orderData == "null")
            {
                //orderData = await GetAll();
                //Console.WriteLine(orderData);
            }
            refetch = true;
            if (!refetch)
            {
                var orders = JsonConvert.DeserializeObject<OrderItem[]>(orderData);

                var orderItems = new List<OrderItem>();
                for (int i = 0; i < orders.Length; i++)
                {
                    if (orders[i].OrderNumber == orderNumber)
                    {
                        orderItems.Add(orders[i]);
                    }
                }

                return orderItems;
            }
            else
            {
                // Only fetch items of this order.
                var orderItems = await PresseroManager.SearchOrderItems(query);

                // disabled the handle Cache function by return orderItems - the old function cannot adapt to the api change
                return orderItems;
                if (orderItems.Any())
                {
                    var orders = JsonConvert.DeserializeObject<List<OrderItem>>(orderData);
                    foreach (var item in orderItems)
                    {
                        bool bExists = false;
                        for (int i = 0; i < orders.Count; i++)
                        {
                            if (item.OrderNumber == orders[i].OrderNumber &&
                                item.OrderItemNumber == orders[i].OrderItemNumber)
                            {
                                bExists = true;

                                orders[i] = item;
                                break;
                            }
                        }

                        // Add it if it is not in our list.
                        if (!bExists)
                        {
                            orders.Add(item);
                            orders.Sort((a, b) =>
                            {
                                return a.OrderNumber.CompareTo(b.OrderNumber);
                            });
                        }
                    }

                    orderData = JsonConvert.SerializeObject(orders);

                    // Save to our cache
                    await _distributedCache.SetStringAsync(
                        CACHE_KEY_PRESSERO_ORDERS, orderData);
                }

                return orderItems;
            }
        }

        private static Status GetStatus(string stageId, DateTime stageDate)
        {
            switch (stageId)
            {
                case "fc5110e1-1c47-4b1c-a977-4dfec763f779":
                    return new Status("SF0010", "Estimate Request", stageDate);
                case "13fd81b1-1b67-4e46-96b6-64207c33716e":
                    return new Status("SF0020", "Estimate Clarification", stageDate);
                case "e0261bc1-8276-4ac4-a456-169bf4be428b":
                    return new Status("SF0030", "Quote", stageDate);
                case "aece99d2-902f-4a49-873a-10f70cdec1c0":
                    return new Status("SF0040", "Quote Acceptance", stageDate);
                case "39167ccd-c859-4d30-b496-bee8f8382f42":
                    return new Status("SF0050", "Order Received", stageDate);
                case "4d03ab0c-5297-454d-ad9a-056ff6f7611d":
                    return new Status("SF0060", "Prepress", stageDate);
                case "7769967f-dc67-492f-8fd1-2cbe0a41f270":
                    return new Status("SF0070", "Press", stageDate);
                case "ba835391-5a41-4e74-a0ec-f2f87e7962c8":
                    return new Status("SF0080", "Bindery", stageDate);
                case "9391a3aa-3082-4125-be88-33c76bcfdc5d":
                    return new Status("SF0090", "Special", stageDate);
                case "bd7816e9-a22a-4c82-bc2d-d1a3c3711821":
                    return new Status("SF0100", "Shipping", stageDate);
                case "2cf71159-8770-4a66-8f43-cc58600cdddd":
                    return new Status("SF0110", "Order Completed", stageDate);
                case "a61679f8-5518-431f-aab0-ba5e10c67203":
                    return new Status("SF0120", "Order Cancelled", stageDate);
                default: return null;
            }
        }

        private async Task CreateAttributes(string site)
        {
            var products = await PresseroManager.GetProductDetails(site);
            _logger.LogInformation("Total products: " + products.Count());

            foreach (var product in products)
            {

                _logger.LogInformation("Notes: " + product.ProductLocationNotes);
                var quoteBillingData = RetrieveQuoteBilling(product);

                if (quoteBillingData.Item2 != eBillingCategory.NONE)
                {
                    var attributes = new Models.ProductAttributeInfo[]
                    {
						// Quote
						new Models.ProductAttributeInfo("7765c2ce-09d1-46e2-b4f2-a7403a65a009", quoteBillingData.Item1.ID),
						// Billing category
						new Models.ProductAttributeInfo("2d7c1ed6-f7ac-4f4b-9a6a-35fa6be33bed", quoteBillingData.Item2.ToString().Replace('_','-')),
                    };

                    var prodWithAttributes = new Product()
                    {
                        ProductInfo = new ProductInfo(),
                        ProductId = product.ProductId,
                        Attributes = attributes
                    };

                    var result = await PresseroManager.UpdateProduct(site, prodWithAttributes);
                    if (!result.IsSuccess)
                    {
                        _logger.LogError("Failed to create attributes. Data {0}. Result {1}.",
                            JsonConvert.SerializeObject(prodWithAttributes),
                            JsonConvert.SerializeObject(result));
                    }
                    else
                    {
                        _logger.LogInformation("Attribues created success");
                    }
                }
                else
                {
                    _logger.LogError("Empty notes, skipping. Data {0}.",
                            JsonConvert.SerializeObject(product));
                }
            }
        }

        public static Tuple<QuoteJob, eBillingCategory> RetrieveQuoteBilling(Product product)
        {
            var quote = new QuoteJob();
            var billCat = eBillingCategory.NONE;

            if (product != null && !string.IsNullOrEmpty(product.ProductLocationNotes))
            {
                string[] data = product.ProductLocationNotes.Split(new[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries);

                if (data != null && data.Length == 2)
                {
                    var quoteUrl = data[0].Split('/');
                    quote.ID = quoteUrl[quoteUrl.Length - 1];

                    Enum.TryParse(data[1].Replace('-', '_'), out billCat);
                }
            }

            return new Tuple<QuoteJob, eBillingCategory>(quote, billCat);
        }

        public static Tuple<eBillingCategory, QuoteJob> RetrieveQuoteBilling(IEnumerable<ProductAttributeInfo> attributes)
        {
            var quote = new QuoteJob() { QuoteJobType = eQuoteJob.QUOTE };
            var billCat = eBillingCategory.NONE;

            if (attributes != null)
            {
                var billingAttr = attributes.Where(a => a.AttributeId ==
                            ProductAttributeInfo.Billing).FirstOrDefault();
                var quoteAttr = attributes.Where(a => a.AttributeId ==
                    ProductAttributeInfo.Quote).FirstOrDefault();

                if (billingAttr != null)
                {
                    Enum.TryParse(billingAttr.AttributeValue.Replace('-', '_'), out billCat);
                }

                if (quoteAttr != null)
                {
                    quote.ID = quoteAttr.AttributeValue;
                }
            }

            return new Tuple<eBillingCategory, QuoteJob>(billCat, quote);
        }

        public async Task UploadInventory(string siteUrl, string filepath)
        {
            using (var reader = new StreamReader(new FileStream(filepath, FileMode.Open)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    int reorderPoint = -1;
                    int currentLevel = -1;

                    int.TryParse(values[1], out reorderPoint);
                    int.TryParse(values[2], out currentLevel);

                    var inventory = new Inventory()
                    {
                        Stock = new Stock()
                        {
                            StockName = values[0],
                            ReorderPoint = reorderPoint,
                        },
                        CurrentLevel = currentLevel
                    };

                    inventory = await PresseroManager.CreateStockInventory(siteUrl, inventory);

                    _logger.LogInformation("INFO-INV: {0},{1}", inventory.Stock.StockName, inventory.Stock.StockId);
                    /*
					File.AppendAllText(
						@"C:\Users\ranjit\Desktop\Pressero\Liberty Medical\15-8-17\Inventory GUIDs.txt",
						string.Format("INFO-INV: {0},{1}", inventory.Stock.StockName, inventory.Stock.StockId));
						*/
                }
            }
        }

        public async Task UploadProducts(string siteUrl, string filepath)
        {
            string outputFilename = "./" + Path.GetFileName(Path.GetTempFileName());

            using (var reader = new StreamReader(new FileStream(filepath, FileMode.Open)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('|');

                    int daysToShipInStock = -1;
                    int daysToShipOutStock = -1;

                    int.TryParse(values[5], out daysToShipOutStock);
                    int.TryParse(values[6], out daysToShipInStock);

                    int minQty = 0;
                    int maxQty = 0;
                    int markupPercent = 0;

                    var markupStr = Regex.Matches(values[13], @"\(([^)]*)\)");

                    if (markupStr.Count == 3)
                    {
                        string val = markupStr[0].Value
                                        .Substring(1, markupStr[0].Value.Length - 2);
                        int.TryParse(markupStr[0].Value
                                        .Substring(1, markupStr[0].Value.Length - 2),
                                        out minQty);
                        int.TryParse(markupStr[1].Value
                                        .Substring(1, markupStr[1].Value.Length - 2), out maxQty);
                        int.TryParse(markupStr[2].Value
                                        .Substring(1, markupStr[2].Value.Length - 2), out markupPercent);
                    }
                    else
                    {
                        _logger.LogError("ERROR-PRDT: {0},{1}", "Invalid markup parameter", values[1]);
                    }

                    var product = new Product()
                    {
                        ProductInfo = new ProductInfo()
                        {
                            //ProductId = values[1],
                            PublicPartNum = values[0],
                            UrlName = values[0],
                            ShortDescription = values[0],
                            IsBackOrderingAllowed = values[2] == "TRUE",
                            ProductName = values[3],
                            DaysToShipInStock = daysToShipInStock,
                            DaysToShipOutStock = daysToShipOutStock,
                            StockId = string.IsNullOrEmpty(values[9]) ? null : values[9],
                            VendorGetsNonMarkupPrice = values[14] == "TRUE",
                            VendorId = values[15],
                            WorkflowId = "d41787d7-57d0-4dcc-ab5a-23df5ff6c1eb"
                        },
                        //Categories = values[4].Split(','),
                        AllowedGroups = values[7].Split(','),
                        ImageUrl = "https://dashboard.epress.com.au/images/storefront/" + values[0].Trim() + ".jpg",
                        Attributes = new ProductAttributeInfo[]
                        {
                            new ProductAttributeInfo()
                            {
                                AttributeId = ProductAttributeInfo.Quote,
                                AttributeValue = values[8].Trim()
                            },
                            new ProductAttributeInfo()
                            {
                                AttributeId = ProductAttributeInfo.Billing,
                                AttributeValue = values[10].Trim()
                            }
                        },
                        ProductGroupCalculators = new ProductGroupCalculatorInfo[]
                        {
                            new ProductGroupCalculatorInfo()
                            {
                                CalculatorId = values[11],
                                GroupId = values[12]
                            }
                        },
                        Markups = new ProductMarkupInfo[]
                        {
                            new ProductMarkupInfo()
                            {
                                MinQty = minQty,
                                MaxQty = maxQty,
                                Markup = markupPercent
                            }
                        }
                    };

                    if (string.IsNullOrEmpty(values[11]))
                    {
                        product.ProductGroupCalculators = null;
                    }

                    if (minQty == 0)
                    {
                        product.Markups = null;
                    }

                    product = await PresseroManager.CreateProduct(siteUrl, product);
                    //var result = await PresseroManager.UpdateProduct(siteUrl, product);
                    //var result = Result.Error("");

                    if (product == null)
                    {
                        _logger.LogInformation("ERROR-PRDT: {0},{1}", product.ProductInfo.ProductId,
                        product.ProductInfo.PublicPartNum);
                    }
                    else
                    {

                        _logger.LogInformation("INFO-PRDT: {0},{1}", product.ProductInfo.ProductId,
                            product.ProductInfo.PublicPartNum);
                        File.AppendAllText(outputFilename, product.ProductInfo.ProductId + ":" +
                            product.ProductInfo.PublicPartNum + Environment.NewLine);
                    }
                }
            }
        }

        public async Task UploadAssets(string siteUrl, string folderPath)
        {
            var tmpFile = "./" + Path.GetFileName(Path.GetTempFileName());

            var productData = await PresseroManager.SearchProducts(siteUrl);
            var assets = await PresseroManager.SearchSubscriberAssets(siteUrl);

            var products = productData.Where(p => !string.IsNullOrEmpty(p.PublicPartNumber)).ToDictionary(p => p.PublicPartNumber.Trim());

            foreach (var file in Directory.GetFiles(folderPath))
            {
                var fileName = Path.GetFileName(file);
                var sku = fileName.Split('_')[0];

                if (assets.Where(a => a.Name.Split(' ')[0].Trim() == sku.Trim()).FirstOrDefault() != null)
                    continue;

                if (products.ContainsKey(sku))
                {
                    var asset = new SubscriberAsset()
                    {
                        Name = sku + " - Click here to download a PDF",
                        Description = Path.GetFileNameWithoutExtension(fileName),
                        Path = "https://dashboard.epress.com.au/images/storefront/" + fileName,
                        UserDownloadable = true,
                        IsLocal = true,
                        AssetType = "2",
                        AssetUsage = "Product",
                        ProductId = products[sku].ProductId,
                        GenerateThumbnailForExternalURL = true
                    };

                    asset = await PresseroManager.CreateSubscriberAsset(siteUrl, asset);
                    if (asset == null)
                    {
                        File.AppendAllText("./" + tmpFile,
                            string.Format("ERROR: Failed to create asset for file {0}",
                            fileName));
                    }
                    else
                    {
                        File.AppendAllText("./" + tmpFile,
                            string.Format("SUCCESS: Asset created for file {0}. Asset Id: {1}",
                            fileName, asset.AssetId));
                    }
                }
                else
                {
                    File.AppendAllText("./" + tmpFile,
                        string.Format("ERROR: Cannot find product for file {0}.", fileName));
                }
            }
        }

        public async Task FixAssetNames(string siteUrl)
        {
            var assets = await PresseroManager.SearchSubscriberAssets(siteUrl);
            foreach (var asset in assets)
            {
                var updatedAsset = new SubscriberAsset()
                {
                    AssetId = asset.AssetId,
                    Name = asset.Name.Split(' ')[0].Trim() + " - Click here to download a PDF"
                };

                var result = await PresseroManager.UpdateSubscriberAsset(siteUrl, updatedAsset);
                if (!result.IsSuccess)
                    _logger.LogError("Failed to update asset {0}", updatedAsset.AssetId);

            }
        }

        public async Task CreateJob(Order order, OrderItem item)
        {
            if (item.IsApproved && !item.IsDenied)
            {
                //var inventoryTransactions = await PresseroManager.GetInventoryTransactions("");
                var billingQuoteData = RetrieveQuoteBilling(item.Product.Attributes);

                var billingCat = billingQuoteData.Item1;
                var quote = billingQuoteData.Item2;

                if (billingCat == eBillingCategory.PRI_BAO ||
                    billingCat == eBillingCategory.PRI_BAODP ||
                    billingCat == eBillingCategory.VDP_BAO ||
                    billingCat == eBillingCategory.VDP_BDP)
                {

                    var quantity = item.Quantity * GetQuantityFromPricingEngine(item.PricingEngineInfo);

                    var job = new QuoteJob()
                    {
                        PreviousQuoteJob = quote,
                        Quantity = new Price() { Quantity = quantity },
                        Required = order.ReqShipDate,
                        Variation = new Variation() { ID = "1" }
                    };

                    // Create a job
                    job = await APIManager.CreateJob(job);

                    if (job != null)
                    {
                        var updateJobNumber = new OrderItem()
                        {
                            OrderNumber = item.OrderNumber,
                            OrderItemNumber = item.OrderItemNumber,
                            JobId = job.ID,
                            JobNumber = job.ID,
                            MisId = job.ID,
                            OrderItemMISID = job.ID
                        };

                        var result = await PresseroManager.UpdateOrderItem(updateJobNumber);
                        if (!result.IsSuccess)
                        {
                            // Notify Admin
                        }
                    }
                }
            }
        }

        private static int GetQuantityFromPricingEngine(string pricingEngineInfo)
        {
            int qty = 0;
            var xml = XDocument.Parse(pricingEngineInfo);

            var options = xml.Element("PricingParms").Element("Options");
            foreach (var option in options.Descendants())
            {
                //Console.WriteLine(option.ToString());
                if (option.HasElements && option.Element("Key") != null &&
                    option.Element("Key").Value == "UOMQty")
                {
                    int.TryParse(option.Element("Value").Value, out qty);
                    break;
                }
            }

            if (qty == 0)
            {
                return 1;
                // Break the fire glass!

            }

            return qty;
        }

        public static DateTime UtcToLocal(DateTime dateTime)
        {
            if (dateTime != null)
            {
                return dateTime.AddHours(10);
            }

            return DateTime.MinValue;
        }
    }
}