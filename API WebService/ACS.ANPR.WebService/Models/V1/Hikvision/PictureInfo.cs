using System.Xml.Serialization;

namespace ACS.ANPR.WebService.Models.V1.Hikvision
{
    public class PictureInfo
    {
        [XmlElement("fileName")]
        public string? FileName { get; set; }

        [XmlElement("type")]
        public string? Type { get; set; }

        [XmlElement("dataType")]
        public int DataType { get; set; }

        [XmlElement("pId")]
        public string? PId { get; set; }

        [XmlElement("absTime")]
        public string? AbsTime { get; set; }

        [XmlElement("plateRect")]
        public Rect? PlateRect { get; set; }

        [XmlElement("vehicelRect")]
        public Rect? VehicelRect { get; set; }

        [XmlElement("PilotRect")]
        public Rect? PilotRect { get; set; }

        [XmlElement("VicepilotRect")]
        public Rect? VicepilotRect { get; set; }

        [XmlElement("VehicelWindowRect")]
        public Rect? VehicelWindowRect { get; set; }

        [XmlElement("capturePicSecurityCode")]
        public string? CapturePicSecurityCode { get; set; }
    }
}
