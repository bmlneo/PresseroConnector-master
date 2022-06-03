using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
	public class EventData
	{
		public int Type { get; set; }
		public string TypeName { get; set; }
		public DateTime Timestamp { get; set; }
		public Order Data { get; set; }
		public string SubscriberId { get; set; }
		public string SiteId { get; set; }
		public string MessageID { get; set; }
	}
}
