using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData.StorkeStyle.StorkeStyles
{
    public class FlaVariablePointWidthRaw
    {
        [XmlAttribute("position")]
        public float Position;

        [XmlAttribute("right")] 
        public float Right;

        [XmlAttribute("left")]
        public float Left;

        [XmlAttribute("type")]
        public string Type;
    }
}