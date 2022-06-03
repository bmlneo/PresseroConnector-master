using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
	public class PresseroStatus
	{
		[Required]
		public string ID { get; set; }
		
		public string Notes { get; set; }

		[Required]
		public IEnumerable<OrderItem> OrderItems { get; set; }

	}
}
