using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
	public class InvoiceQuery
	{
		public string SiteUrl { get; set; }
		public string StageId { get; set; }

		public DateTime From { get; set; }
		public DateTime To { get; set; }

        public string OrderNumber { get; set; }


	}
}
