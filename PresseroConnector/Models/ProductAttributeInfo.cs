using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class ProductAttributeInfo
    {
		public const string Billing = "2d7c1ed6-f7ac-4f4b-9a6a-35fa6be33bed";
		public const string Quote = "7765c2ce-09d1-46e2-b4f2-a7403a65a009";

		public ProductAttributeInfo()
		{

		}

		public ProductAttributeInfo(string id, string value)
		{
			AttributeId = id;
			AttributeValue = value;
		}

		public string AttributeId { get; set; }
		public string AttributeValue { get; set; }

	}
}
