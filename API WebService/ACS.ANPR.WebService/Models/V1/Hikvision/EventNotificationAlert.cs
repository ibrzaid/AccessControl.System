using System.Xml.Serialization;

namespace ACS.ANPR.WebService.Models.V1.Hikvision
{
    [XmlRoot("EventNotificationAlert", Namespace = "http://www.isapi.org/ver20/XMLSchema")]
    public class EventNotificationAlert
    {
        [XmlAttribute("version")]
        public string? Version { get; set; }

        [XmlElement("ipAddress")]
        public string? IpAddress { get; set; }

        [XmlElement("ipv6Address")]
        public string? Ipv6Address { get; set; }

        [XmlElement("protocol")]
        public string? Protocol { get; set; }

        [XmlElement("macAddress")]
        public string? MacAddress { get; set; }

        [XmlElement("dynChannelID")]
        public int DynChannelID { get; set; }

        [XmlElement("channelID")]
        public int ChannelID { get; set; }

        [XmlElement("dateTime")]
        public string? DateTime { get; set; }

        [XmlElement("activePostCount")]
        public int ActivePostCount { get; set; }

        [XmlElement("eventType")]
        public string? EventType { get; set; }

        [XmlElement("eventState")]
        public string? EventState { get; set; }

        [XmlElement("eventDescription")]
        public string? EventDescription { get; set; }

        [XmlElement("channelName")]
        public string? ChannelName { get; set; }

        [XmlElement("deviceID")]
        public string? DeviceID { get; set; }

        [XmlElement("ANPR")]
        public ANPR? ANPR { get; set; }

        [XmlElement("UUID")]
        public string? UUID { get; set; }

        [XmlElement("picNum")]
        public int PicNum { get; set; }

        [XmlElement("monitoringSiteID")]
        public string? MonitoringSiteID { get; set; }

        [XmlElement("monitorDescription")]
        public string? MonitorDescription { get; set; }

        [XmlElement("DeviceGPSInfo")]
        public DeviceGPSInfo? DeviceGPSInfo { get; set; }

        [XmlElement("carDirectionType")]
        public int CarDirectionType { get; set; }

        [XmlElement("deviceUUID")]
        public string? DeviceUUID { get; set; }

        [XmlElement("VehicleGATInfo")]
        public VehicleGATInfo? VehicleGATInfo { get; set; }

        [XmlElement("detectionBackgroundImageResolution")]
        public Resolution? DetectionBackgroundImageResolution { get; set; }
    }
}
