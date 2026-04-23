using System.Xml.Serialization;

namespace ACS.ANPR.WebService.Models.V1.Hikvision
{
    public class DeviceGPSInfo
    {
        [XmlElement("longitudeType")]
        public string? LongitudeType { get; set; }

        [XmlElement("latitudeType")]
        public string? LatitudeType { get; set; }

        [XmlElement("Longitude")]
        public Coordinate? Longitude { get; set; }

        [XmlElement("Latitude")]
        public Coordinate? Latitude { get; set; }
    }
}
