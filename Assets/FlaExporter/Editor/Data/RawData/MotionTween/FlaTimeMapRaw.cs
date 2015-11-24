using System.Xml.Serialization;

namespace Assets.FlaExporter.Editor.Data.RawData.MotionTween
{
    public class FlaTimeMapRaw
    {
        //strength="0" type="Quadratic"
        [XmlAttribute("strength")]
        public float Strength;

        [XmlAttribute("type")]
        public string Type;

        [XmlElement("PropertyContainer")]
        public FlaPropertyContainerRaw PropertyContainer;
    }
}