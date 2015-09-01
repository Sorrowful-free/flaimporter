using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.MotionTween
{
    public class FlaAnimationCoreRaw
    {
        [XmlAttribute("TimeScale")]
        public int TimeScale;

        [XmlAttribute("Version")]
        public int Version;

        [XmlAttribute("duration")]
        public int Duration;

        [XmlElement("TimeMap")]
        public FlaTimeMapRaw TimeMap;
    }
}