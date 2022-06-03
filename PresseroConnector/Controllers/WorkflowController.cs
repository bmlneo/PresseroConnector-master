using Microsoft.AspNetCore.Mvc;
using PresseroConnector.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Controllers
{
	[Route("api/[controller]")]
	public class WorkflowController : Controller
	{
		[HttpGet, Route("{id}")]
		public async Task<string> Get(string id)
		{
			return await PresseroManager.GetWorkflowsById(id);
		}
	}
}
