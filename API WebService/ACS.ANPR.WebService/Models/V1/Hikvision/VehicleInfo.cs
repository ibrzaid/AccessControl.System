using System.Xml.Serialization;

namespace ACS.ANPR.WebService.Models.V1.Hikvision
{
    public class VehicleInfo
    {
        [XmlElement("index")]
        public int Index { get; set; }

        [XmlElement("vehicleType")]
        public int VehicleType { get; set; }

        [XmlElement("colorDepth")]
        public int ColorDepth { get; set; }

        [XmlElement("color")]
        public string? Color { get; set; }

        [XmlElement("speed")]
        public int Speed { get; set; }

        [XmlElement("length")]
        public int Length { get; set; }

        [XmlElement("vehicleLogoRecog")]
        public int VehicleLogoRecog { get; set; }

        [XmlElement("vehileSubLogoRecog")]
        public int VehileSubLogoRecog { get; set; }

        [XmlElement("vehileModel")]
        public int VehileModel { get; set; }

        [XmlElement("CarWindowFeature")]
        public CarWindowFeature? CarWindowFeature { get; set; }

        [XmlElement("CarBodyFeature")]
        public CarBodyFeature? CarBodyFeature { get; set; }

        [XmlElement("vehicleUseType")]
        public string? VehicleUseType { get; set; }
    }
}
