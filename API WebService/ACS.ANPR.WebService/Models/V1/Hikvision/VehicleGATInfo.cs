using System.Xml.Serialization;

namespace ACS.ANPR.WebService.Models.V1.Hikvision
{
    public class VehicleGATInfo
    {
        [XmlElement("palteTypeByGAT")]
        public int PlateTypeByGAT { get; set; }

        [XmlElement("plateColorByGAT")]
        public int PlateColorByGAT { get; set; }

        [XmlElement("vehicleTypeByGAT")]
        public string? VehicleTypeByGAT { get; set; }

        [XmlElement("colorByGAT")]
        public string? ColorByGAT { get; set; }
    }
}
