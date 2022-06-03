using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
	public class SortDescriptor
	{
		public SortDescriptor() { }

		public SortDescriptor(string member, string direction)
		{
			Member = member;
			Direction = direction;
		}

		public string Member { get; set; }
		public string Direction { get; set; }
	}

	public class FilterExpression
	{
		public FilterExpression() { }

		public FilterExpression(string column, string value, string op)
		{
			Column = column;
			Value = value;
            Operator = op;

        }

		public string Column { get; set; }
		public string Value { get; set; }

        public string Operator { get; set; }
    }

	public class OrderItemQuery
	{
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public SortDescriptor SortDescriptor { get; set; }
		public FilterExpression[] Filters { get; set; }
	}
}
