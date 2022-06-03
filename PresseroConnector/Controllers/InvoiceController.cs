using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PresseroConnector.Models;
using PresseroConnector.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using PresseroConnector.Managers;

namespace PresseroConnector.Controllers
{
	[Route("api/[controller]")]
	public class InvoiceController : Controller
	{
		private readonly ILogger _logger;
		private readonly IDistributedCache _distributedCache;

		private OrderDAL context;

		public InvoiceController(ILogger<InvoiceController> logger, IDistributedCache distributedCache)
		{
			_logger = logger;
			_distributedCache = distributedCache;

			context = new OrderDAL(_logger, _distributedCache);
		}

		// GET: api/values
		[HttpPost]
		public async Task<IActionResult> Get([FromBody]InvoiceQuery query)
		{
			if (query.To < query.From)
				return BadRequest();

            

			var queryStartDate = query.From.AddMonths(-6);
			var queryEndDate = query.From.AddMonths(-3);

			var allOrderItems = new Dictionary<(int, int), OrderItem>();

			while (queryEndDate.Date < DateTime.Now.Date)
			{
                if (query.SiteUrl.StartsWith("ceav"))
                {
                    queryStartDate = query.From.AddDays(-14);
                    queryEndDate = query.To;
                }
                else
                {

                    queryStartDate = queryEndDate;
                    queryEndDate = queryEndDate.AddMonths(3);
                }

				var filters = new FilterExpression[]
				{
					new FilterExpression()
					{
						Column = "WorkflowStageId",
						Value = query.StageId,
                        Operator = "equals"
                    }
				};
                IEnumerable<OrderItem> orderItems;
                try
                {
                    orderItems = await context.Get(query.SiteUrl, queryStartDate, queryEndDate, filters);
                }
                catch(Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }

				foreach (var orderItem in orderItems)
				{
					if (!allOrderItems.ContainsKey((orderItem.OrderNumber, orderItem.OrderItemNumber)))
					{
						allOrderItems.Add((orderItem.OrderNumber, orderItem.OrderItemNumber), orderItem);
					}
				}
                if (query.SiteUrl.StartsWith("ceav"))
                {
                    queryEndDate = DateTime.Now;
                }

            }

			var orderItemsCompleted = new List<OrderItem>();
			foreach (var orderItem in allOrderItems)
			{
				if (orderItem.Value.WorkflowStageDate.Date >= query.From.Date &&
					orderItem.Value.WorkflowStageDate.Date <= query.To.Date)
					orderItemsCompleted.Add(orderItem.Value);
			}

			return Ok(orderItemsCompleted);
		}

        [HttpGet, Route("ShippingMethods")]
        public async Task<IActionResult> GetShippingMethods()
        {

            var result = await PresseroManager.GetShippingMethods();

            if (result == null)
                return BadRequest();
            else
                return Ok(result);
        }

        [HttpGet, Route("ShippingMethodDetails/{id}")]
        public async Task<IActionResult> GetShippingMethodDetails(string id)
        {

            var result = await PresseroManager.GetShippingMethodDetails(id);

            if (result == null)
                return BadRequest();
            else
                return Ok(result);
        }
    }
}
