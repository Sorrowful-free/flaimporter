using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData.MotionTween
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