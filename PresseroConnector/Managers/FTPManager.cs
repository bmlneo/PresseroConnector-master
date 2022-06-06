using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PresseroConnector.Models;
using System.Net;
using System.Text;
using System.IO;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Net.Http;
using Newtonsoft.Json;

namespace PresseroConnector.Managers
{
    public static class FTPManager
    {
        class OrderCSV
        {
            public string Name { get; set; }
            public string Data { get; set; }
        }

        private const string HOST = "13.77.44.12";
        private const string USERNAME = "shopify";
        private const string PASSWORD = "pVSPp6j34G";

        public static async Task UploadOrderItemToNM(ILogger _logger, IEnumerable<OrderItem> items)
        {
            var siteId = items.First().SiteId;


            //apply receivercode for convatec?

            // Nichemark's internal reference.
            var receiverCode = Constants.NM_RECEIVER_CODE_LIBERTY_MEDICAL;
            switch (siteId)
            {
                case Constants.PEDDERS_SITE_ID:
                    receiverCode = Constants.NM_RECEIVER_CODE_PEDDERS;
                    break;
                case Constants.FPAA_SITE_ID:
                    receiverCode = Constants.NM_RECEIVER_CODE_FPAA;
                    break;
                case Constants.CONVATEC_SITE_ID:
                    receiverCode = Constants.NM_RECEIVER_CODE_CONVATEC;
                    break;
                case Constants.HTAV_SITE_ID:
                    receiverCode = Constants.NM_RECEIVER_CODE_HTAV;
                    break;
                case Constants.HIGGINS_SITE_ID:
                    receiverCode = Constants.NM_RECEIVER_CODE_HIGGINS;
                    break;
                case Constants.SMEC_SITE_ID:
                    receiverCode = Constants.NM_RECEIVER_CODE_SMEC;
                    break;
                case Constants.DAHLSENS_SITE_ID:
                    receiverCode = Constants.NM_RECEIVER_CODE_DAHLSENS;
                    break;
                case Constants.COLLINS_SITE_ID:
                    receiverCode = Constants.NM_RECEIVER_CODE_COLLINS;
                    break;
                case Constants.FULTON_SITE_ID:
                    receiverCode = Constants.NM_RECEIVER_CODE_FULTON;
                    break;
                case Constants.CATHSUPER_SITE_ID:
                    receiverCode = Constants.NM_RECEIVER_CODE_CATHSUPER;
                    break;
                case Constants.NDY_SITE_ID:
                    receiverCode = Constants.NM_RECEIVER_CODE_NDY;
                    break;
                case Constants.RHALF_SITE_ID:
                    receiverCode = Constants.NM_RECEIVER_CODE_RHALF;
                    break;
                case Constants.GADENS_SITE_ID:
                    receiverCode = Constants.NM_RECEIVER_CODE_GADENS;
                    break;

            }

            StringBuilder sb = new StringBuilder();

            _logger.LogInformation("Grouping items by shipping location");
            var itemsGroupedByShipTo = items.GroupBy(i => i.ShipToAddress1 + i.ShipToAddress2 + i.ShipToAddress3 + i.ShipToCity + i.ShipToPostal);

            // Header
            sb.Append("Debtor Code,Warehouse,Customer Reference,Job Reference,");
            sb.Append("Receiver Code,Receiver Name,Receiver Address,");
            sb.Append("Receiver Suburb,Receiver Postcode,Receiver State,");
            sb.Append("Receiver Telephone,Receiver Contact,Product Code,");
            sb.Append("Ordered Qty,Line Note,Order Date,Delivery Date,");
            sb.Append("Instructions");



            // Only submit if we have any approved lines.
            bool bSubmit = false;

            int index = 0;
            foreach (var group in itemsGroupedByShipTo)
            {
                index++;
                foreach (var item in group)
                {
                    if (!item.IsApproved || item.IsDenied)
                        continue;

                    bSubmit = true;


                    string dCode = "EP";


                    if (receiverCode == "FPAA")
                    { dCode = "FP"; }


                    sb.Append(Environment.NewLine);
                    sb.AppendFormat("{4},BRN-MUL,EP {0}-{1},,{3},", //{0}-{2}
                        item.OrderNumber, index, item.OrderItemNumber, receiverCode, dCode);


                    // If ship to business name is empty, use the receiver contact name instead.
                    var receiverName = string.IsNullOrEmpty(item.ShipToBusiness) ? item.ShipToFirstName + " " + item.ShipToLastName : item.ShipToBusiness;

                    if (receiverName == "") //if receiverName is blank, default to C/O RECEPTION
                    { receiverName = "C/O RECEPTION"; }

                    sb.AppendFormat("{0},{1} {2} {3},{4},{5},{6},{7},{8} {9},",
                        Upper(receiverName),
                        Upper(item.ShipToAddress1), Upper(item.ShipToAddress2),
                        Upper(item.ShipToAddress3), Upper(item.ShipToCity),
                        Upper(item.ShipToPostal), Upper(item.ShiptoStateProvince),
                        Upper(item.ShipToPhone),
                        Upper(item.ShipToFirstName), Upper(item.ShipToLastName));

                    string pCode = "";
                    if (item.ClientPartNumber == "" || item.ClientPartNumber == null)
                    {
                        pCode = item.PrinterPartNumber;
                    }
                    else
                    { pCode = item.ClientPartNumber; }

                    sb.AppendFormat("{0},{1},{6}-{7},{3},{4},{8}",
                        pCode, item.Quantity.ToString(),
                        "", item.OrderDate.ToString("ddMMyy"),
                        (item.RequestedShipDate != null && item.RequestedShipDate.HasValue) ?
                            item.RequestedShipDate.Value.ToString("ddMMyy") : "",
                        Upper(item.ItemNotes), item.OrderNumber, item.OrderItemNumber, (item.ShippingMethod == "Customer Pick Up") ? "Customer Pick Up" : "");

                }
            }

            _logger.LogInformation("Data: {0}", sb.ToString());
            if (!bSubmit)
            {
                _logger.LogInformation("No lines to submit.");
                return;
            }
            string fileName = "";

            fileName = string.Format("Order-{0}-{1}.csv", items.First().OrderNumber,
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));


            string contentStr = sb.ToString();

            var contentData = Encoding.UTF8.GetBytes(contentStr);

            _logger.LogInformation("Sending email...");

            var emailSubject = string.Format("New Order: #{0} from {1}", items.First().OrderNumber, items.First().SiteName);


            const int MAX_ATTEMPTS = 10;
            int attempts = 0;

            //await Utility.EmailCSVFile(fileName, emailSubject, contentData);
            //File.WriteAllText(@"/var/www/pending-nm-pressero-orders/" + fileName, contentStr);


            try
            {
                File.WriteAllText(@"/var/www/pending-nm-pressero-orders/" + fileName, contentStr);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\nFILENAME: {fileName}");
            }

            _logger.LogInformation($"FILE: {fileName} created.");

            while (true)
            {
                if (++attempts > MAX_ATTEMPTS)
                {
                    _logger.LogError($"Maximum failed attempts ({MAX_ATTEMPTS}) reached attempting to send email.");
                    break;
                }

                try
                {
                    await Utility.EmailCSVFile(fileName, emailSubject, contentData);
                    break;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    await Utility.NotifyAdmin("PresseroConnector Error", e.Message);
                }
            }

            // Create FtpWebRequest object 
            //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(
            //    string.Format("ftp://{0}/Incoming/{1}", HOST, fileName));

            //request.Method = WebRequestMethods.Ftp.UploadFile;
            //request.EnableSsl = false;
            //request.UsePassive = false;
            //request.Credentials = new NetworkCredential(USERNAME, PASSWORD);
            //request.ContentLength = contentData.Length;

            //try
            //{
            //	// Write data to stream
            //	Stream requestSteam = await request.GetRequestStreamAsync();
            //	requestSteam.Write(contentData, 0, contentData.Length);
            //	requestSteam.Close();

            //	FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync();
            //	if (response.StatusCode != FtpStatusCode.ClosingData)
            //	{
            //		await Utility.NotifyAdmin("Pressero Connector: Failed to upload order to FTP",
            //		string.Format("<p><b>Error<b><br>{0}</p><p><b>Order data<b><br>{1}</p>",
            //		"Error Status: " + response.StatusCode + " " + response.StatusDescription,
            //		sb.ToString()));
            //	}

            //	response.Close();
            //}
            //catch (Exception ex)
            //{
            //	await Utility.NotifyAdmin("Pressero Connector: Failed to upload order to FTP",
            //		string.Format("<p><b>Error<b><br>{0}</p><p><b>Order data<b><br>{1}</p>",
            //		ex.ToString(),
            //		sb.ToString()));
            //}
        }
        private static string ReplaceNewlines(string blockOfText, string replaceWith = " ")
        {
            return blockOfText != null ? blockOfText.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith) : "";
        }
        private static string Upper(string str)
        {
            str = ReplaceNewlines(str);
            return str != null ? str.ToUpper().Replace(",", "") : "";
        }
    }
}