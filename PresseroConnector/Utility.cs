using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PresseroConnector.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using MailKit.Net.Smtp;
using MimeKit;
using System.IO;
using System.Text;
using MailKit.Security;
using Newtonsoft.Json.Linq;
using System.Security.Authentication;
using MailKit.Net.Imap;
using MailKit;

namespace PresseroConnector
{
	public class Utility
	{
		public const string APPLICATIONJSON = "application/json";
		public const string FORMURLENCODED = "application/x-www-form-urlencoded";

		private const string ADMINEMAIL = "programming@epress.com.au";

		private static HttpClient httpClient = new HttpClient();

		public static async Task<Result> CreateWebRequest(Uri uri, HttpMethod method, string body,
												string authorisation, string contentType = "application/json")
		{
			var request = new HttpRequestMessage(method, uri);
			if (!string.IsNullOrEmpty(authorisation))
			{
				request.Headers.Add("Authorization", authorisation);
			}

			if (!string.IsNullOrEmpty(body))
			{
				request.Content = new StringContent(body);
				request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
			}
            if (uri.ToString().Contains("api/site/ceav.impressiveonline.com.au/Orders"))
            {
                httpClient.Timeout = TimeSpan.FromMinutes(5);
            }
			var response = await httpClient.SendAsync(request);
			var responseString = await response.Content.ReadAsStringAsync();

           

			if (response.IsSuccessStatusCode)
			{
				// by calling .Result you are performing a synchronous call
				return Result.Success(responseString);
			}

			return Result.Error(response.StatusCode.ToString() + Environment.NewLine + responseString);
		}

		public static T ParseJSON<T>(string json)
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(json);
			}
			catch (Exception e)
			{
				Console.WriteLine("Error parsing JSON: " + e.ToString());
			}

			return default(T);
		}

		public static async Task NotifyAdmin(string subject, string body)
		{
			await SendEmail(
				new MailboxAddress[] { new MailboxAddress(ADMINEMAIL) },
				null,
				subject,
				body,
				null);
		}

		public static async Task SendEmail(MailboxAddress[] to, MailboxAddress[] cc, string subject,
			string body, IEnumerable<MimeEntity> attachments)
		{
			var mail = new MimeMessage();
			mail.From.Add(new MailboxAddress("Eastern Press", "update@epress.com.au"));
			mail.To.AddRange(to);
			if (cc != null)
				mail.Cc.AddRange(cc);

			mail.Subject = subject;

			var bodyBuilder = new BodyBuilder();
			if (!string.IsNullOrEmpty(body))
				bodyBuilder.HtmlBody = "<!DOCTYPE html><html><head></head><body>" + body + "</body></html>";

			if (attachments != null)
			{
				foreach (var attachment in attachments)
				{
					bodyBuilder.Attachments.Add(attachment);
				}
			}

			//bodyBuilder.Attachments.Add(name, data, new ContentType("text/csv", "csv"));

			mail.Body = bodyBuilder.ToMessageBody();

			using (var client = new SmtpClient())
			{
				client.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);

				client.Authenticate("update@epress.com.au", "update@EP1!");

				await client.SendAsync(mail);
				client.Disconnect(true);
			}
		}

		public static async Task EmailCSVFile(string name, string subject, byte[] data)
		{
            var mail = new MimeMessage();
			mail.From.Add(new MailboxAddress("Eastern Press", "update@epress.com.au"));
            mail.To.Add(new MailboxAddress("kieran@nichemark.com.au"));
            mail.Cc.Add(new MailboxAddress("cormac@epress.com.au"));
            mail.Cc.Add(new MailboxAddress("liam@epress.com.au"));
			mail.Cc.Add(new MailboxAddress("programming@epress.com.au"));
            mail.Cc.Add(new MailboxAddress("clare@epress.com.au"));

            mail.Subject = subject;

			var bodyBuilder = new BodyBuilder();
			bodyBuilder.Attachments.Add(name, data, new ContentType("text/csv", "csv"));

			mail.Body = bodyBuilder.ToMessageBody();

			using (var client = new SmtpClient(new ProtocolLogger(@"nf-smtp.log")))
			{
                client.SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;

                client.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);

				client.Authenticate("update@epress.com.au", "update@EP1!");

				await client.SendAsync(mail);
				client.Disconnect(true);
			}
		}

        //while (true)
        //{
        //    if (++attempts > MAX_ATTEMPTS)
        //    {
        //        _logger.LogError($"Maximum failed attempts (${MAX_ATTEMPTS}) reached attempting to send email.");
        //        break;
        //    }

        //    try
        //    {
        //        await Utility.EmailCSVFile(fileName, emailSubject, contentData);
        //        File.WriteAllText(@"/var/www/pending-nm-pressero-orders/" + fileName, contentStr);
        //        break;
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e.Message);
        //    }
        //}

        public static void WriteToLog(string msg)
        {
            string path = @"var\www\PresseroConnector\nf-log\ENTRY-" + String.Format("{0:MMMM-yyyy}", DateTime.Now) + ".txt";
            //string path = System.IO.Path.Combine(@"C:\nick\testing\EPAPI\log", "ERRORS-" + String.Format("{0:MMMM-yyyy}", DateTime.Now) + ".txt");

            try
            {
                byte[] encodedText = Encoding.ASCII.GetBytes(String.Format("{0:t}", DateTime.Now) + ": " + msg + "\n");
                using (FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false))
                {
                    fileStream.Write(encodedText, 0, encodedText.Length);
                };

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
    }
}
