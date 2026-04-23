using System.Xml.Serialization;

namespace ACS.ANPR.WebService.Models.V1.Hikvision
{
    public class CarWindowFeature
    {
        [XmlElement("tempPlate")]
        public string? TempPlate { get; set; }

        [XmlElement("passCard")]
        public string? PassCard { get; set; }

        [XmlElement("carCard")]
        public string? CarCard { get; set; }
    }
}
