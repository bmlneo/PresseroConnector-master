using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class InventoryTransaction
    {
		public string TransactionId { get; set; }
		public DateTime Date { get; set; }
		public string Type { get; set; }
		public int TotalQuantity { get; set; }
	}

	public class InventoryTransactionPagination
	{
		public IEnumerable<InventoryTransaction> Items;
	}
}