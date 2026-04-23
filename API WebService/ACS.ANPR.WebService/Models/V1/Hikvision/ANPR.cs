using System.Xml.Serialization;

namespace ACS.ANPR.WebService.Models.V1.Hikvision
{
    public class ANPR
    {
        [XmlElement("country")]
        public string? Country { get; set; }

        [XmlElement("licensePlate")]
        public string? LicensePlate { get; set; }

        [XmlElement("line")]
        public int Line { get; set; }

        [XmlElement("direction")]
        public string? Direction { get; set; }

        [XmlElement("confidenceLevel")]
        public int ConfidenceLevel { get; set; }

        [XmlElement("plateType")]
        public string? PlateType { get; set; }

        [XmlElement("plateColor")]
        public string? PlateColor { get; set; }

        [XmlElement("licenseBright")]
        public int LicenseBright { get; set; }

        [XmlElement("pilotsafebelt")]
        public string? PilotSafebelt { get; set; }

        [XmlElement("vicepilotsafebelt")]
        public string? VicepilotSafebelt { get; set; }

        [XmlElement("pilotsunvisor")]
        public string? PilotSunvisor { get; set; }

        [XmlElement("vicepilotsunvisor")]
        public string? VicepilotSunvisor { get; set; }

        [XmlElement("envprosign")]
        public string? Envprosign { get; set; }

        [XmlElement("dangmark")]
        public string? Dangmark { get; set; }

        [XmlElement("uphone")]
        public string? Uphone { get; set; }

        [XmlElement("pendant")]
        public string? Pendant { get; set; }

        [XmlElement("tissueBox")]
        public string? TissueBox { get; set; }

        [XmlElement("frontChild")]
        public string? FrontChild { get; set; }

        [XmlElement("label")]
        public string? Label { get; set; }

        [XmlElement("smoking")]
        public string? Smoking { get; set; }

        [XmlElement("perfumeBox")]
        public string? PerfumeBox { get; set; }

        [XmlElement("decoration")]
        public string? Decoration { get; set; }

        [XmlElement("playMobilePhone")]
        public string? PlayMobilePhone { get; set; }

        [XmlElement("pdvs")]
        public string? Pdvs { get; set; }

        [XmlElement("helmet")]
        public string? Helmet { get; set; }

        [XmlElement("nonMotorShedUmbrella")]
        public string? NonMotorShedUmbrella { get; set; }

        [XmlElement("nonMotorManned")]
        public string? NonMotorManned { get; set; }

        [XmlElement("pilotmask")]
        public string? PilotMask { get; set; }

        [XmlElement("vicepilotMask")]
        public string? VicepilotMask { get; set; }

        [XmlElement("plateCharBelieve")]
        public string? PlateCharBelieve { get; set; }

        [XmlElement("speedLimit")]
        public int SpeedLimit { get; set; }

        [XmlElement("illegalInfo")]
        public IllegalInfo? IllegalInfo { get; set; }

        [XmlElement("vehicleType")]
        public string? VehicleType { get; set; }

        [XmlElement("featurePicFileName")]
        public string? FeaturePicFileName { get; set; }

        [XmlElement("detectDir")]
        public int DetectDir { get; set; }

        [XmlElement("relaLaneDirectionType")]
        public int RelaLaneDirectionType { get; set; }

        [XmlElement("detectType")]
        public int DetectType { get; set; }

        [XmlElement("barrierGateCtrlType")]
        public int BarrierGateCtrlType { get; set; }

        [XmlElement("alarmDataType")]
        public int AlarmDataType { get; set; }

        [XmlElement("dwIllegalTime")]
        public int DwIllegalTime { get; set; }

        [XmlElement("vehicleInfo")]
        public VehicleInfo? VehicleInfo { get; set; }

        [XmlElement("pictureInfoList")]
        public PictureInfoList? PictureInfoList { get; set; }

        [XmlElement("listType")]
        public string? ListType { get; set; }

        [XmlElement("originalLicensePlate")]
        public string? OriginalLicensePlate { get; set; }

        [XmlElement("CRIndex")]
        public string? CRIndex { get; set; }

        [XmlElement("plateCategory")]
        public string? PlateCategory { get; set; }

        [XmlElement("plateSize")]
        public int PlateSize { get; set; }
    }
}
