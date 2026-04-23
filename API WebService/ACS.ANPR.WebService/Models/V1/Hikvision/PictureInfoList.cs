using System.Xml.Serialization;

namespace ACS.ANPR.WebService.Models.V1.Hikvision
{
    public class PictureInfoList
    {
        [XmlElement("pictureInfo")]
        public List<PictureInfo>? PictureInfos { get; set; }
    }
}
