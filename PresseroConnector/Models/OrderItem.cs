using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PresseroConnector.Models
{
	public class OrderItem
	{
		public string OrderId { get; set; }
		public int OrderNumber { get; set; }

		public string OrderItemId { get; set; }
		public int OrderItemNumber { get; set; }
		public int OrderItemSeq { get; set; }

        public DateTime orderDate;
        public DateTime OrderDate/* { get; set; }*/
        {
            get { return orderDate; }
            set
            {
                DateTime utcDate = value;
                orderDate = utcDate.ToLocalTime();
            }
        }

        public DateTime? approvedDate;
        public DateTime? ApprovedDate
        {
            get { return approvedDate; }
            set
            {
                if (value == null)
                    approvedDate = value;
                else
                {
                    DateTime utcDate = (DateTime)value;
                    approvedDate = utcDate.ToLocalTime();
                }

            }
        }

        public DateTime? requestedShipDate;
        public DateTime? RequestedShipDate
        {
            get { return requestedShipDate; }
            set
            {
                if (value == null)
                    requestedShipDate = value;
                else
                {
                    DateTime utcDate = (DateTime) value;
                    requestedShipDate = utcDate.ToLocalTime();
                }
                
            }
        }

		public Product Product { get; set; }
		public string ProductName { get; set; }

		public string ItemName { get; set; }
		
		public string OrderItemOptionsText { get;set; }

		public int Quantity { get; set; }
		public int UOMQuantity { get; set; }
		public int TotalQuantity { get; set; }

		public float Price { get; set; }
		public string PricingEngineInfo { get; set; }
		public float Shipping { get; set; }

		public string SiteId { get; set; }
		public string SiteName { get; set; }
		public string Domain { get; set; }

		public string SalesRepFirstName { get; set; }
		public string SalesRepLastName { get; set; }
		public string SalesRepEmail { get; set; }
		
		public string OrderedByFirstName { get; set; }
		public string OrderedByLastName { get; set; }
		public string OrderedByEmail { get; set; }
		public string OrderedByDepartment { get; set; }

		public string PONumber { get; set; }
		
		public string BillTo { get; set; }
		public string BillToFirstName { get; set; }
		public string BillToLastName { get; set; }
		public string BillToBusiness { get; set; }
		public string BillToAddress1 { get; set; }
		public string BillToAddress2 { get; set; }
		public string BillToAddress3 { get; set; }
		public string BillToCity { get; set; }
		public string BillToPostal { get; set; }
		public string BilltoStateProvince { get; set; }
		public string BillToCountry { get; set; }
		public string BillToPhone { get; set; }
		public string BillToEmail { get; set; }

		public string ShippingMethodId { get; set; }
		public string ShippingMethod { get; set; }
		public DateTime? ShipmentDate { get; set; }
		public string ShipmentTracking { get; set; }
		public float ShipmentCost { get; set; }

		public string ShipTo { get; set; }
		public string ShipToFirstName { get; set; }
		public string ShipToLastName { get; set; }
		public string ShipToBusiness { get; set; }
		public string ShipToAddress1 { get; set; }
		public string ShipToAddress2 { get; set; }
		public string ShipToAddress3 { get; set; }
		public string ShipToCity { get; set; }
		public string ShipToPostal { get; set; }
		public string ShipToCountry { get; set; }
		public string ShiptoStateProvince { get; set; }
		public string ShipToPhone { get; set; }

		public string ItemNotes { get; set; }

		public string WorkflowId { get; set; }
		public string WorkflowStageId { get; set; }

        public DateTime workflowStageDate;
        public DateTime WorkflowStageDate
        {
            get { return workflowStageDate; }
            set
            {
                DateTime utcDate = value;
                workflowStageDate = utcDate.ToLocalTime();
            }
        }
        public int WorkflowStageNumber { get; set; }
		public string WorkflowStagePrinter { get; set; }
		public string WorkflowStageCustomer { get; set; }

		public bool IsPaid { get; set; }
		public bool IsApproved { get; set; }
		public bool IsDenied { get; set; }
		public bool IsApprovedWithModifications { get; set; }

		public float? OrderSubtotal { get; set; }
		public float? OrderShipping { get; set; }
		public float? OrderTax { get; set; }
		public float? OrderDiscount { get; set; }
		public float? OrderTotal { get; set; }
		public float? ItemTax { get; set; }

		public string JobId { get; set; }
		public string JobNumber { get; set; }
		public string MisId { get; set; }
		public string OrderItemMISID { get; set; }
				
		public string PrinterPartNumber { get; set; }
		public string ClientPartNumber { get; set; }

		public IEnumerable<ProductAttributeInfo> Attributes { get; set; }

		public string ProductAttributeName1 { get; set; }
		public string ProductAttributeValue1 { get; set; }
		public string ProductAttributeName2 { get; set; }
		public string ProductAttributeValue2 { get; set; }
		public string ProductAttributeName3 { get; set; }
		public string ProductAttributeValue3 { get; set; }
		public string ProductAttributeName4 { get; set; }
		public string ProductAttributeValue4 { get; set; }
		public string ProductAttributeName5 { get; set; }
		public string ProductAttributeValue5 { get; set; }
		public string ProductAttributeName6 { get; set; }
		public string ProductAttributeValue6 { get; set; }
		public string ProductAttributeName7 { get; set; }
		public string ProductAttributeValue7 { get; set; }
		public string ProductAttributeName8 { get; set; }
		public string ProductAttributeValue8 { get; set; }
		public string ProductAttributeName9 { get; set; }
		public string ProductAttributeValue9 { get; set; }
		public string ProductAttributeName10 { get; set; }
		public string ProductAttributeValue10 { get; set; }

        public string[] GetAttributeNamesAsArray()
        {
            return new[]
            {
                ProductAttributeName1, ProductAttributeName2,
                ProductAttributeName3, ProductAttributeName4,
                ProductAttributeName5, ProductAttributeName6,
                ProductAttributeName7, ProductAttributeName8,
                ProductAttributeName9, ProductAttributeName10
            };
        }

        public string[] GetAttributeValuesAsArray()
        {
            return new[] {
                ProductAttributeValue1, ProductAttributeValue2,
                ProductAttributeValue3, ProductAttributeValue4,
                ProductAttributeValue5, ProductAttributeValue6,
                ProductAttributeValue7, ProductAttributeValue8,
                ProductAttributeValue9, ProductAttributeValue10
            };
        }

		public string ProductFormField1 { get; set; }
		public string ProductFormValue1 { get; set; }
		public string ProductFormField2 { get; set; }
		public string ProductFormValue2 { get; set; }
		public string ProductFormField3 { get; set; }
		public string ProductFormValue3 { get; set; }
		public string ProductFormField4 { get; set; }
		public string ProductFormValue4 { get; set; }
		public string ProductFormField5 { get; set; }
		public string ProductFormValue5 { get; set; }
		public string ProductFormField6 { get; set; }
		public string ProductFormValue6 { get; set; }
		public string ProductFormField7 { get; set; }
		public string ProductFormValue7 { get; set; }
		public string ProductFormField8 { get; set; }
		public string ProductFormValue8 { get; set; }
		public string ProductFormField9 { get; set; }
		public string ProductFormValue9 { get; set; }
		public string ProductFormField10 { get; set; }
		public string ProductFormValue10 { get; set; }
        public string CheckoutFormField1 { get; set; }
        public string CheckoutFormValue1 { get; set; }

    }

	public class OrderItemData
	{
		public OrderItem[] Items { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
	}
}
