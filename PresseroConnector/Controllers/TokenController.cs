using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PresseroConnector.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresseroConnector.Controllers
{
	[Route("api/[controller]")]
	public class TokenController : Controller
	{
		private readonly ILogger _logger;
		
		public TokenController(ILogger<OrderController> logger)
		{
			_logger = logger;
		}

		[HttpPost, AllowAnonymous]
		public async Task<IActionResult> Token([FromBody]ApplicationUser user)
		{
			if (user == null || !ModelState.IsValid)
			{
				return BadRequest();
			}

			if (user.Username != "pressero@epress.com.au" || user.Password != "n]n5'!f5-E.")
				return BadRequest();

			var token = GetJwtSecurityToken(user);

			return Ok(new
			{
				token = new JwtSecurityTokenHandler().WriteToken(token),
				expiration = token.ValidTo
			});
		}

		private JwtSecurityToken GetJwtSecurityToken(ApplicationUser user)
		{
			return new JwtSecurityToken(
				issuer: "pressero.epress.com.au",
				audience: "pressero.epress.com.au",
				expires: DateTime.UtcNow.AddDays(10),
				signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("RiOdumFOLX8wv2iy3F01L8qzoqAgMOycauXXIX2DQ5f40ZXY7VylU3RgJpfT0wBdBlpgnzPnzbfZFfifdw59qQ==")), SecurityAlgorithms.HmacSha256)
			);
		}
	}
}
