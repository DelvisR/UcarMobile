using NetTopologySuite.Geometries;

namespace UcarMobileApi.Core.Entities.Common;

public class AddressInfo
{
    public string FullAddress { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;

    public Point BasePoint { get; set; } = null!;
    public double Lat => BasePoint.Y;
    public double Lng => BasePoint.X;
}
