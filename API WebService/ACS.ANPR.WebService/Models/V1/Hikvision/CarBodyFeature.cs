using System.Xml.Serialization;

namespace ACS.ANPR.WebService.Models.V1.Hikvision
{
    public class CarBodyFeature
    {
        [XmlElement("sparetire")]
        public string? Sparetire { get; set; }

        [XmlElement("rack")]
        public string? Rack { get; set; }

        [XmlElement("sunRoof")]
        public string? SunRoof { get; set; }

        [XmlElement("words")]
        public string? Words { get; set; }

        [XmlElement("reflectiveStripe")]
        public string? ReflectiveStripe { get; set; }
    }
}
