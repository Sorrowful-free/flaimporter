using System.Xml.Serialization;

namespace Assets.FlaExporter.Editor.Data.RawData.ShapeTween
{
    public class FlaMorphCurvesRaw
    {
        [XmlAttribute("controlPointA")]
        public string ControlPointA;

        [XmlAttribute("anchorPointA")]
        public string AnchorPointA;

        [XmlAttribute("controlPointB")]
        public string ControlPointB;

        [XmlAttribute("anchorPointB")]
        public string AnchorPointB;

        [XmlAttribute("isLine")]
        public bool IsLine;
    }
}