using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class Status
    {
		public Status() { }

		public Status(string id, string name, DateTime updated)
		{
			ID = id;
			Name = name;
			Updated = updated;
		}

		public string ID { get; set; }
		public string Name { get; set; }
		public DateTime Updated { get; set; }
	}
}
