using System.Xml.Serialization;

namespace ACS.ANPR.WebService.Models.V1.Hikvision
{
    public class IllegalInfo
    {
        [XmlElement("illegalCode")]
        public int IllegalCode { get; set; }

        [XmlElement("illegalName")]
        public string? IllegalName { get; set; }

        [XmlElement("illegalDescription")]
        public string? IllegalDescription { get; set; }
    }
}
