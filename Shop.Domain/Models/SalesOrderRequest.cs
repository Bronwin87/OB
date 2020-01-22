//    using Shop.Domain.Models;
//
//    var SalesOrder = SalesOrder.FromJson(jsonString);

namespace Shop.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.JsonPatch;
    

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class SalesOrder
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("createdDate")]
        public DateTimeOffset CreatedDate { get; set; }

        [JsonProperty("modifiedDate")]
        public DateTimeOffset ModifiedDate { get; set; }

        [JsonProperty("createdBy")]
        public long CreatedBy { get; set; }

        [JsonProperty("processedBy")]
        public long ProcessedBy { get; set; }

        [JsonProperty("isApproved")]
        public bool IsApproved { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("memberId")]
        public long MemberId { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("fax")]
        public string Fax { get; set; }

        [JsonProperty("deliveryFirstName")]
        public string DeliveryFirstName { get; set; }

        [JsonProperty("deliveryLastName")]
        public string DeliveryLastName { get; set; }

        [JsonProperty("deliveryCompany")]
        public string DeliveryCompany { get; set; }

        [JsonProperty("deliveryAddress1")]
        public string DeliveryAddress1 { get; set; }

        [JsonProperty("deliveryAddress2")]
        public string DeliveryAddress2 { get; set; }

        [JsonProperty("deliveryCity")]
        public string DeliveryCity { get; set; }

        [JsonProperty("deliveryState")]
        public string DeliveryState { get; set; }

        [JsonProperty("deliveryPostalCode")]
        public string DeliveryPostalCode { get; set; }

        [JsonProperty("deliveryCountry")]
        public string DeliveryCountry { get; set; }

        [JsonProperty("billingFirstName")]
        public string BillingFirstName { get; set; }

        [JsonProperty("billingLastName")]
        public string BillingLastName { get; set; }

        [JsonProperty("billingCompany")]
        public string BillingCompany { get; set; }

        [JsonProperty("billingAddress1")]
        public string BillingAddress1 { get; set; }

        [JsonProperty("billingAddress2")]
        public string BillingAddress2 { get; set; }

        [JsonProperty("billingCity")]
        public string BillingCity { get; set; }

        [JsonProperty("billingPostalCode")]
        public string BillingPostalCode { get; set; }

        [JsonProperty("billingState")]
        public string BillingState { get; set; }

        [JsonProperty("billingCountry")]
        public string BillingCountry { get; set; }

        [JsonProperty("branchId")]
        public long BranchId { get; set; }

        [JsonProperty("branchEmail")]
        public string BranchEmail { get; set; }

        [JsonProperty("projectName")]
        public string ProjectName { get; set; }

        [JsonProperty("trackingCode")]
        public string TrackingCode { get; set; }

        [JsonProperty("internalComments")]
        public string InternalComments { get; set; }

        [JsonProperty("productTotal")]
        public long ProductTotal { get; set; }

        [JsonProperty("freightTotal")]
        public long FreightTotal { get; set; }

        [JsonProperty("freightDescription")]
        public string FreightDescription { get; set; }

        [JsonProperty("surcharge")]
        public long Surcharge { get; set; }

        [JsonProperty("surchargeDescription")]
        public string SurchargeDescription { get; set; }

        [JsonProperty("discountTotal")]
        public long DiscountTotal { get; set; }

        [JsonProperty("discountDescription")]
        public string DiscountDescription { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("currencyCode")]
        public string CurrencyCode { get; set; }

        [JsonProperty("currencyRate")]
        public long CurrencyRate { get; set; }

        [JsonProperty("currencySymbol")]
        public string CurrencySymbol { get; set; }

        [JsonProperty("taxStatus")]
        public string TaxStatus { get; set; }

        [JsonProperty("taxRate")]
        public long TaxRate { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("customFields")]
        public CustomFields CustomFields { get; set; }

        [JsonProperty("memberEmail")]
        public string MemberEmail { get; set; }

        [JsonProperty("memberCostCenter")]
        public string MemberCostCenter { get; set; }

        [JsonProperty("memberAlternativeTaxRate")]
        public string MemberAlternativeTaxRate { get; set; }

        [JsonProperty("costCenter")]
        public string CostCenter { get; set; }

        [JsonProperty("alternativeTaxRate")]
        public string AlternativeTaxRate { get; set; }

        [JsonProperty("estimatedDeliveryDate")]
        public DateTimeOffset EstimatedDeliveryDate { get; set; }

        [JsonProperty("salesPersonId")]
        public long SalesPersonId { get; set; }

        [JsonProperty("salesPersonEmail")]
        public string SalesPersonEmail { get; set; }

        [JsonProperty("paymentTerms")]
        public string PaymentTerms { get; set; }

        [JsonProperty("customerOrderNo")]
        public string CustomerOrderNo { get; set; }

        [JsonProperty("voucherCode")]
        public string VoucherCode { get; set; }

        [JsonProperty("deliveryInstructions")]
        public string DeliveryInstructions { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("stage")]
        public string Stage { get; set; }

        [JsonProperty("invoiceDate")]
        public DateTimeOffset InvoiceDate { get; set; }

        [JsonProperty("invoiceNumber")]
        public long InvoiceNumber { get; set; }

        [JsonProperty("dispatchedDate")]
        public DateTimeOffset DispatchedDate { get; set; }

        [JsonProperty("logisticsCarrier")]
        public string LogisticsCarrier { get; set; }

        [JsonProperty("logisticsStatus")]
        public long LogisticsStatus { get; set; }

        [JsonProperty("lineItems")]
        public LineItem[] LineItems { get; set; }
    }

    public partial class CustomFields
    {
        [JsonProperty("sample string 1")]
        public SampleString SampleString1 { get; set; }

        [JsonProperty("sample string 3")]
        public SampleString SampleString3 { get; set; }
    }

    public partial class SampleString
    {
    }

    public partial class LineItem
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("createdDate")]
        public DateTimeOffset CreatedDate { get; set; }

        [JsonProperty("transactionId")]
        public long TransactionId { get; set; }

        [JsonProperty("productId")]
        public long ProductId { get; set; }

        [JsonProperty("productOptionId")]
        public long ProductOptionId { get; set; }

        [JsonProperty("integrationRef")]
        public string IntegrationRef { get; set; }

        [JsonProperty("sort")]
        public long Sort { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("option1")]
        public string Option1 { get; set; }

        [JsonProperty("option2")]
        public string Option2 { get; set; }

        [JsonProperty("option3")]
        public string Option3 { get; set; }

        [JsonProperty("qty")]
        public long Qty { get; set; }

        [JsonProperty("styleCode")]
        public string StyleCode { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        [JsonProperty("sizeCodes")]
        public string SizeCodes { get; set; }

        [JsonProperty("lineComments")]
        public string LineComments { get; set; }

        [JsonProperty("unitCost")]
        public long UnitCost { get; set; }

        [JsonProperty("unitPrice")]
        public long UnitPrice { get; set; }

        [JsonProperty("discount")]
        public long Discount { get; set; }

        [JsonProperty("qtyShipped")]
        public long QtyShipped { get; set; }

        [JsonProperty("holdingQty")]
        public long HoldingQty { get; set; }

        [JsonProperty("accountCode")]
        public string AccountCode { get; set; }
    }

    public partial class SalesOrder
    {
        public static SalesOrder[] FromJson(string json) => JsonConvert.DeserializeObject<SalesOrder[]>(json, Shop.Domain.Models.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this SalesOrder[] self) => JsonConvert.SerializeObject(self, Shop.Domain.Models.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
