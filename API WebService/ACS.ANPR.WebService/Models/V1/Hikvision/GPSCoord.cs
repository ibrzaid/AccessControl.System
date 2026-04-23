using System.Xml.Serialization;

namespace ACS.ANPR.WebService.Models.V1.Hikvision
{
    public class GPSCoord
    {
        [XmlElement("degree")] public int Degree { get; set; }
        [XmlElement("minute")] public int Minute { get; set; }
        [XmlElement("sec")] public double Sec { get; set; }
    }
}
