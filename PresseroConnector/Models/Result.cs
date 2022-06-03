using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
	public class Result
	{
		public static Result Success(object data = null)
		{
			return new Result() { IsSuccess = true, Data = data };
		}

		public static Result Error(string message)
		{
			return new Result() { IsSuccess = false, Message = message };
		}

		public bool IsSuccess { get; set; }
		public string Message { get; set; }
		public object Data { get; set; }
	}
}
