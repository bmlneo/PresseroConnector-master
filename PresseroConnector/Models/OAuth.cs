using System;
using Newtonsoft.Json;

namespace PresseroConnector.Models
{
	public class OAuth
	{
		[JsonProperty("access_token")]
		public string Token { get; set; }
		[JsonProperty("token_type")]
		public string Type { get; set; }
		[JsonProperty("expires_in")]
		public int ExpiresIn { get; set; }

		public DateTime Created { get; set; } = DateTime.Now;

		public override string ToString()
		{
			return string.Format("{0} {1}", Type, Token);
		}
	}
}
