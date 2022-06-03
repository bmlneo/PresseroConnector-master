using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class ApplicationUser
    {
		[Required]
		public string Username { get; set; }
		[Required]
		public string Password { get; set; }
    }
}
