using System.Xml.Serialization;

namespace ACS.ANPR.WebService.Models.V1.Hikvision
{
    public class Resolution
    {
        [XmlElement("height")]
        public int Height { get; set; }

        [XmlElement("width")]
        public int Width { get; set; }
    }
}
