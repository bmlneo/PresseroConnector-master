using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class Inventory
    {
		public Stock Stock { get; set; }
		public int CurrentLevel { get; set; }
    }
}
