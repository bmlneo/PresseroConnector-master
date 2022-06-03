using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class Entity
    {
		public string ID { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }

		public string Address { get; set; }
		public string Suburb { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Postcode { get; set; }
		public string Country { get; set; }
	}
}
