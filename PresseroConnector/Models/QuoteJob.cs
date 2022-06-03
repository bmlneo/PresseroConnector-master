using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
	public enum eQuoteJob
	{
		QUOTE = 0,
		JOB,
		NONE
	}

	public class Variation
	{
		public string ID { get; set; }
	}

	public class QuoteJob
	{
		public string ID { get; set; }
		public string Name { get; set; }

		public eQuoteJob QuoteJobType { get; set; }

		public DateTime Created { get; set; }
		public DateTime? Required { get; set; }

		public QuoteJob Quote { get; set; }

		public Variation Variation { get; set; }

		public Status Status { get; set; }

		public Entity Customer { get; set; }
		public QuoteJob PreviousQuoteJob { get; set; }
		public Price Quantity { get; set; }
		public string Suburb { get; set; }
		
		public OrderItem OrderItem { get; set; }

		public string Note { get; set; }
	}
}
