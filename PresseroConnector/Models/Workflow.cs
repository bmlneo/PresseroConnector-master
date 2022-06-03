using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class Workflow
    {
		public string WorkflowStageId { get; set; }
		public string Seq { get; set; }

		public string PrinterStage { get; set; }
		public string CustomerStage { get; set; }
		public string Description { get; set; }
	}
}
