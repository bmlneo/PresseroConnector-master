using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PresseroConnector.Models;

namespace PresseroConnector.Managers
{
	public class APIManager
	{
		#region Constants
		private const string URL = "http://localhost:59695/";
		//private const string USERNAME = "test@epress.com.au";
		//private const string PASSWORD = "rEqepe=Has4w";
		private const string USERNAME = "production@epress.com.au";
		private const string PASSWORD = "production@EP1!";
		#endregion

		private static OAuth oAuth = null;
		
		public static async Task<QuoteJob> CreateJob(QuoteJob job)
		{
			var result = await Utility.CreateWebRequest(
				new Uri(URL + "Job/Copy"),
				HttpMethod.Post,
				JsonConvert.SerializeObject(job),
				await GetToken());

			if (result.IsSuccess)
				return JsonConvert.DeserializeObject<QuoteJob>((string)result.Data);

			return null;
		}
		
		private static async Task<string> GetToken()
		{
			if (oAuth != null && oAuth.Created.AddSeconds(oAuth.ExpiresIn) > DateTime.Now.AddHours(1))
				return oAuth.ToString();

			var result = await Utility.CreateWebRequest(new Uri(URL + "token"), HttpMethod.Post,
				string.Format("grant_type=password&username={0}&password={1}", USERNAME, PASSWORD),
				null, Utility.FORMURLENCODED);

			if (result.IsSuccess)
			{
				oAuth = Utility.ParseJSON<OAuth>((string)result.Data);
				if (oAuth != null)
					return oAuth.ToString();
			}

			return null;
		}
	}
}
