using System.Collections.Generic;
using Newtonsoft.Json;

namespace UcarMobileApi.Infrastructure.Configurations.AutoZone.Models
{
    public class OrderDto
    {
        [JsonProperty("orderDetail")]
        public OrderDetail? OrderDetail { get; set; }

        [JsonProperty("lineItems")]
        public List<LineItems>? LineItems { get; set; }

        [JsonProperty("customerPin")]
        public string? CustomerPin { get; set; }

        [JsonProperty("sourceId")]
        public string? SourceId { get; set; }
    }

    public class Status
    {
        [JsonProperty("state")]
        public string? State { get; set; }

        [JsonProperty("timeStamp")]
        public string? TimeStamp { get; set; }

        [JsonProperty("code")]
        public string? Code { get; set; }

        [JsonProperty("text")]
        public string? Text { get; set; }
    }

    public class Adjustments
    {
        [JsonProperty("adjustmentCode")]
        public string? AdjustmentCode { get; set; }

        [JsonProperty("adjustmentDescription")]
        public string? AdjustmentDescription { get; set; }

        [JsonProperty("adjustmentAmount")]
        public double? AdjustmentAmount { get; set; }

        [JsonProperty("parentName")]
        public string? ParentName { get; set; }

        [JsonProperty("parentId")]
        public string? ParentId { get; set; }
    }

    public class MclStats
    {
        [JsonProperty("timestamp")]
        public string? Timestamp { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }
    }

    public class MclInfo
    {
        [JsonProperty("updateAvailable")]
        public string? UpdateAvailable { get; set; }

        [JsonProperty("mclStats")]
        public MclStats? MclStats { get; set; }
    }

    public class OrderDetail
    {
        [JsonProperty("quoteId")]
        public string? QuoteId { get; set; }

        [JsonProperty("serviceWriter")]
        public string? ServiceWriter { get; set; }

        [JsonProperty("purchaseOrderNumber")]
        public string? PurchaseOrderNumber { get; set; }

        [JsonProperty("deliveryRequest")]
        public string? DeliveryRequest { get; set; }

        [JsonProperty("shipWhole")]
        public string? ShipWhole { get; set; }

        [JsonProperty("noteToStore")]
        public string? NoteToStore { get; set; }

        [JsonProperty("status")]
        public Status? Status { get; set; }

        [JsonProperty("statusHistory")]
        public string? StatusHistory { get; set; }

        [JsonProperty("adjustments")]
        public List<Adjustments>? Adjustments { get; set; }

        [JsonProperty("vdpOrder")]
        public string? VdpOrder { get; set; }

        [JsonProperty("estimateDeliveryDate")]
        public string? EstimateDeliveryDate { get; set; }

        [JsonProperty("employeeId")]
        public string? EmployeeId { get; set; }

        [JsonProperty("tenderType")]
        public string? TenderType { get; set; }

        [JsonProperty("mclInfo")]
        public MclInfo? MclInfo { get; set; }

        [JsonProperty("store")]
        public string? Store { get; set; }

        [JsonProperty("pnaType")]
        public string? PnaType { get; set; }

        [JsonProperty("renderId")]
        public string? RenderId { get; set; }

        [JsonProperty("commercialUID")]
        public string? CommercialUid { get; set; }

        [JsonProperty("beaconOrder")]
        public string? BeaconOrder { get; set; }

        [JsonProperty("dealSavings")]
        public string? DealSavings { get; set; }

        [JsonProperty("cart")]
        public bool Cart { get; set; }
    }

    public class StockNumber
    {
        [JsonProperty("catalogId")]
        public string? CatalogId { get; set; }

        [JsonProperty("catalogName")]
        public string? CatalogName { get; set; }

        [JsonProperty("lineCode")]
        public string? LineCode { get; set; }

        [JsonProperty("partNumber")]
        public string? PartNumber { get; set; }

        [JsonProperty("skuNumber")]
        public string? SkuNumber { get; set; }
    }

    public class CatalogId
    {
        [JsonProperty("stockNumber")]
        public StockNumber? StockNumber { get; set; }

        [JsonProperty("lineCodePartNumber")]
        public string? LineCodePartNumber { get; set; }
    }

    public class VehicleInfo
    {
        [JsonProperty("vehicleId")]
        public string? VehicleId { get; set; }

        [JsonProperty("engineId")]
        public string? EngineId { get; set; }

        [JsonProperty("vehicleDescription")]
        public string? VehicleDescription { get; set; }

        [JsonProperty("engineDescription")]
        public string? EngineDescription { get; set; }
    }

    public class AcesVehicle
    {
        [JsonProperty("country")]
        public string? Country { get; set; }

        [JsonProperty("year")]
        public string? Year { get; set; }

        [JsonProperty("regionId")]
        public string? RegionId { get; set; }

        [JsonProperty("makeId")]
        public string? MakeId { get; set; }

        [JsonProperty("modelId")]
        public string? ModelId { get; set; }

        [JsonProperty("subModelId")]
        public string? SubModelId { get; set; }

        [JsonProperty("driveTypeId")]
        public string? DriveTypeId { get; set; }

        [JsonProperty("engineBaseId")]
        public string? EngineBaseId { get; set; }

        [JsonProperty("engineVinId")]
        public string? EngineVinId { get; set; }

        [JsonProperty("aspirationId")]
        public string? AspirationId { get; set; }

        [JsonProperty("fuelTypeId")]
        public string? FuelTypeId { get; set; }

        [JsonProperty("fuelDeliveryTypeId")]
        public string? FuelDeliveryTypeId { get; set; }

        [JsonProperty("fuelDeliverySubTypeId")]
        public string? FuelDeliverySubTypeId { get; set; }

        [JsonProperty("cylinderHeadTypeId")]
        public string? CylinderHeadTypeId { get; set; }

        [JsonProperty("vehicleTypeId")]
        public string? VehicleTypeId { get; set; }

        [JsonProperty("vehicleTypeGroupId")]
        public string? VehicleTypeGroupId { get; set; }

        [JsonProperty("bedLengthId")]
        public string? BedLengthId { get; set; }

        [JsonProperty("bedTypeId")]
        public string? BedTypeId { get; set; }

        [JsonProperty("bodyNumDoorsId")]
        public string? BodyNumDoorsId { get; set; }

        [JsonProperty("bodyTypeId")]
        public string? BodyTypeId { get; set; }

        [JsonProperty("brakeAbsId")]
        public string? BrakeAbsId { get; set; }

        [JsonProperty("brakeSystemId")]
        public string? BrakeSystemId { get; set; }

        [JsonProperty("electricControlId")]
        public string? ElectricControlId { get; set; }

        [JsonProperty("engineBoreStrokeId")]
        public string? EngineBoreStrokeId { get; set; }

        [JsonProperty("engineDesignationId")]
        public string? EngineDesignationId { get; set; }

        [JsonProperty("engineMfrId")]
        public string? EngineMfrId { get; set; }

        [JsonProperty("engineVersionId")]
        public string? EngineVersionId { get; set; }

        [JsonProperty("frontBrakeTypeId")]
        public string? FrontBrakeTypeId { get; set; }

        [JsonProperty("frontSpringTypeId")]
        public string? FrontSpringTypeId { get; set; }

        [JsonProperty("fuelSystemControlTypeId")]
        public string? FuelSystemControlTypeId { get; set; }

        [JsonProperty("fuelSystemDesignId")]
        public string? FuelSystemDesignId { get; set; }

        [JsonProperty("ignitionSystemTypeId")]
        public string? IgnitionSystemTypeId { get; set; }

        [JsonProperty("mfrBodyCodeId")]
        public string? MfrBodyCodeId { get; set; }

        [JsonProperty("classId")]
        public string? ClassId { get; set; }

        [JsonProperty("rearBrakeTypeId")]
        public string? RearBrakeTypeId { get; set; }

        [JsonProperty("rearSpringTypeId")]
        public string? RearSpringTypeId { get; set; }

        [JsonProperty("steeringSystemId")]
        public string? SteeringSystemId { get; set; }

        [JsonProperty("steeringTypeId")]
        public string? SteeringTypeId { get; set; }

        [JsonProperty("transmissionBaseId")]
        public string? TransmissionBaseId { get; set; }

        [JsonProperty("transmissionControlTypeId")]
        public string? TransmissionControlTypeId { get; set; }

        [JsonProperty("transmissionElectricControlId")]
        public string? TransmissionElectricControlId { get; set; }

        [JsonProperty("transmissionMfrCodeId")]
        public string? TransmissionMfrCodeId { get; set; }

        [JsonProperty("transmissionMfrId")]
        public string? TransmissionMfrId { get; set; }

        [JsonProperty("transmissionNumSpeedsId")]
        public string? TransmissionNumSpeedsId { get; set; }

        [JsonProperty("transmissionTypeId")]
        public string? TransmissionTypeId { get; set; }

        [JsonProperty("valvesId")]
        public string? ValvesId { get; set; }

        [JsonProperty("wheelBaseId")]
        public string? WheelBaseId { get; set; }

        [JsonProperty("powerOutputId")]
        public string? PowerOutputId { get; set; }
    }

    public class StoreAddress
    {
        [JsonProperty("postalAddressLine1LTX")]
        public string? PostalAddressLine1LTX { get; set; }

        [JsonProperty("postalAddressLine2LTX")]
        public string? PostalAddressLine2LTX { get; set; }

        [JsonProperty("cityLNM")]
        public string? CityLNM { get; set; }

        [JsonProperty("stateProvinceCD")]
        public string? StateProvinceCD { get; set; }

        [JsonProperty("postalCD")]
        public string? PostalCD { get; set; }

        [JsonProperty("countryCD")]
        public string? CountryCD { get; set; }

        [JsonProperty("timeZone")]
        public string? TimeZone { get; set; }
    }

    public class Stores
    {
        [JsonProperty("storeNumber")]
        public string? StoreNumber { get; set; }

        [JsonProperty("storeQuantity")]
        public int StoreQuantity { get; set; }

        [JsonProperty("storeAddress")]
        public StoreAddress? StoreAddress { get; set; }

        [JsonProperty("storeDistanceInMiles")]
        public double StoreDistanceInMiles { get; set; }
    }

    public class AvailabilityLocations
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("stores")]
        public List<Stores>? Stores { get; set; }
    }

    public class CoreInfo
    {
        [JsonProperty("coreCost")]
        public int CoreCost { get; set; }

        [JsonProperty("coreList")]
        public int CoreList { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("immediateChargeQty")]
        public int ImmediateChargeQty { get; set; }

        [JsonProperty("coreDeferFlag")]
        public string? CoreDeferFlag { get; set; }

        [JsonProperty("itemFulfillmentNumber")]
        public string? ItemFulfillmentNumber { get; set; }

        [JsonProperty("itemOriginatingStore")]
        public string? ItemOriginatingStore { get; set; }
    }

    public class LineItems
    {
        [JsonProperty("clientPartItemId")]
        public string? ClientPartItemId { get; set; }

        [JsonProperty("catalogId")]
        public CatalogId? CatalogId { get; set; }

        [JsonProperty("partNote")]
        public string? PartNote { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("vehicleInfo")]
        public VehicleInfo? VehicleInfo { get; set; }

        [JsonProperty("acesVehicle")]
        public AcesVehicle? AcesVehicle { get; set; }

        [JsonProperty("partDescription")]
        public string? PartDescription { get; set; }

        [JsonProperty("listPrice")]
        public double ListPrice { get; set; }

        [JsonProperty("shopCost")]
        public double ShopCost { get; set; }

        [JsonProperty("availability")]
        public string? Availability { get; set; }

        [JsonProperty("availabilityLocations")]
        public List<AvailabilityLocations>? AvailabilityLocations { get; set; }

        [JsonProperty("deliveryInfo")]
        public string? DeliveryInfo { get; set; }

        [JsonProperty("partAdjustments")]
        public List<object>? PartAdjustments { get; set; }

        [JsonProperty("status")]
        public Status? Status { get; set; }

        [JsonProperty("statusHistory")]
        public string? StatusHistory { get; set; }

        [JsonProperty("coreInfo")]
        public CoreInfo? CoreInfo { get; set; }

        [JsonProperty("isVdp")]
        public string? IsVdp { get; set; }

        [JsonProperty("estimateDeliveryDate")]
        public string? EstimateDeliveryDate { get; set; }

        [JsonProperty("priceOverrideAmt")]
        public string? PriceOverrideAmt { get; set; }

        [JsonProperty("overrideEmployeeId")]
        public string? OverrideEmployeeId { get; set; }

        [JsonProperty("retailSubCd")]
        public string? RetailSubCd { get; set; }

        [JsonProperty("competitorPriceOverrideCd")]
        public string? CompetitorPriceOverrideCd { get; set; }

        [JsonProperty("competitorOverrideId")]
        public string? CompetitorOverrideId { get; set; }

        [JsonProperty("otherCompetitorName")]
        public string? OtherCompetitorName { get; set; }

        [JsonProperty("creditCardNumber")]
        public string? CreditCardNumber { get; set; }

        [JsonProperty("creditCardExpiration")]
        public string? CreditCardExpiration { get; set; }

        [JsonProperty("creditCardOnFileFlag")]
        public string? CreditCardOnFileFlag { get; set; }

        [JsonProperty("processAboveStoreFlag")]
        public string? ProcessAboveStoreFlag { get; set; }

        [JsonProperty("orderSubtotal")]
        public double OrderSubtotal { get; set; }

        [JsonProperty("orderTaxTotal")]
        public string? OrderTaxTotal { get; set; }

        [JsonProperty("orderInvoiceTotal")]
        public string? OrderInvoiceTotal { get; set; }

        [JsonProperty("itemEnvFee")]
        public string? ItemEnvFee { get; set; }

        [JsonProperty("itemEnvDeposit")]
        public string? ItemEnvDeposit { get; set; }

        [JsonProperty("cashTenderAmount")]
        public string? CashTenderAmount { get; set; }

        [JsonProperty("seqId")]
        public string? SeqId { get; set; }

        [JsonProperty("cutOffTimestamp")]
        public string? CutOffTimestamp { get; set; }

        [JsonProperty("deliveryDateBeforeCutoff")]
        public string? DeliveryDateBeforeCutoff { get; set; }

        [JsonProperty("deliveryDateAfterCutoff")]
        public string? DeliveryDateAfterCutoff { get; set; }

        [JsonProperty("stockingConsignmentFlag")]
        public string? StockingConsignmentFlag { get; set; }

        [JsonProperty("stockingConsignmentDiscount")]
        public string? StockingConsignmentDiscount { get; set; }

        [JsonProperty("dealsInfo")]
        public string? DealsInfo { get; set; }
    }
}
