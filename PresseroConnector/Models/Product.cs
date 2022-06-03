using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
	public enum eBillingCategory : byte
	{
		NONE,
		INV_BAO, // INVENTORY BILL AS ORDERED
		INV_BAODP, // INVENTORY BILL AS ORDERED DIFFERENT PRICE
		INV_PP, // INVENTORY PREPAID
		PRI_BAO, // PRINT ON DEMAND BILL AS ORDERED
		PRI_BAODP, // PRINT ON DEMAND BILL AS ORDERED DIFFERENT PRICE
		VDP_BAO, // VARIABLE DATA ITEM BILL AS ORDERED
		VDP_BDP // VARIABLE DATA ITEM BILL AS ORDERED DIFFERENT PRICE
	}
	

	public class Product
	{
		public string MisId { get; set; }
		public string PrintersPartNum { get; set; }
		public string ProductId { get; set; }
		public string ProductName { get; set; }
		public string ProductLocationNotes { get; set; }

		public IEnumerable<ProductAttributeInfo> Attributes { get; set; }

		public ProductInfo ProductInfo { get; set; }
		
		//public string[] Categories { get; set; }

		public string[] AllowedGroups { get; set; }

		public string ImageUrl { get; set; }

		public ProductMarkupInfo[] Markups { get; set; }

		public ProductGroupCalculatorInfo[] ProductGroupCalculators { get; set; }
	}

	public class ProductInfo
	{
		public string ProductId { get; set; }
		public string ProductName { get; set; }

		public string PublicPartNum { get; set; }
		public string PublicPartNumber { get; set; }
		public string PrintersPartNum { get; set; }

		public string UrlName { get; set; }

		public string ShortDescription { get; set; }
		public string LongDescription { get; set; }

		public string StockId { get; set; }

		public bool? IsBackOrderingAllowed { get; set; }
		
		public int? DaysToShipInStock { get; set; }
		public int? DaysToShipOutStock { get; set; }

		public string WorkflowId { get; set; }

		public string VendorId { get; set; }
		public bool? VendorGetsNonMarkupPrice { get; set; }
        public string SiteName { get; set; }
    }

	public class ProductData
	{
		public ProductInfo[] Items { get; set; }
	}

}
