using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData.IKTree
{
    public class FlaIkStateRaw
    {
        [XmlAttribute("duration")]
        public int Duration;

        [XmlAttribute("angle")]
        public float Angle;

        [XmlAttribute("x")]
        public float X;

        [XmlAttribute("y")]
        public float Y;
    }
}