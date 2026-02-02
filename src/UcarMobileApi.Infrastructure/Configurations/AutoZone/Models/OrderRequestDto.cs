using System.Collections.Generic;
using Newtonsoft.Json;

namespace UcarMobileApi.Infrastructure.Configurations.AutoZone.Models
{
    public class OrderRequestDto
    {
        [JsonProperty("customerPin")]
        public string? CustomerPin { get; set; }

        [JsonProperty("lineItems")]
        public List<LineItemsRequest>? LineItems { get; set; }

        [JsonProperty("orderDetail")]
        public OrderDetailRequest?   OrderDetail { get; set; }

        [JsonProperty("sourceId")]
        public string? SourceId { get; set; }
    }

    public class LineCodePartNumber
    {
        [JsonProperty("lineCode")]
        public string? LineCode { get; set; }

        [JsonProperty("partNumber")]
        public string? PartNumber { get; set; }
    }

    public class CatalogIdRequest
    {
        [JsonProperty("lineCodePartNumber")]
        public LineCodePartNumber? LineCodePartNumber { get; set; }
    }

    public class CoreInfoRequest
    {
        [JsonProperty("coreCost")]
        public int CoreCost { get; set; }

        [JsonProperty("coreList")]
        public int CoreList { get; set; }

        [JsonProperty("immediateChargeQty")]
        public int ImmediateChargeQty { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }

    public class StatusRequest
    {
        [JsonProperty("state")]
        public string? State { get; set; }

        [JsonProperty("timeStamp")]
        public string? TimeStamp { get; set; }
    }

    public class LineItemsRequest
    {
        [JsonProperty("catalogId")]
        public CatalogIdRequest? CatalogId { get; set; }

        [JsonProperty("clientPartItemId")]
        public string? ClientPartItemId { get; set; }

        [JsonProperty("coreInfo")]
        public CoreInfoRequest? CoreInfo { get; set; }

        [JsonProperty("listPrice")]
        public int ListPrice { get; set; }

        [JsonProperty("partDescription")]
        public string? PartDescription { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("shopCost")]
        public int ShopCost { get; set; }

        [JsonProperty("status")]
        public StatusRequest? Status { get; set; }
    }

    public class OrderDetailRequest
    {
        [JsonProperty("deliveryRequest")]
        public string? DeliveryRequest { get; set; }

        [JsonProperty("shipWhole")]
        public string? ShipWhole { get; set; }

        [JsonProperty("status")]
        public StatusRequest? Status { get; set; }
    }
}
