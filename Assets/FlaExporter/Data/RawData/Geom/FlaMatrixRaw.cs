using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.Geom
{
    public class FlaMatrixRaw
    {
        [XmlAttribute("a")]
        public float A;
        [XmlAttribute("b")]
        public float B;
        [XmlAttribute("c")]
        public float C;
        [XmlAttribute("d")]
        public float D;
        [XmlAttribute("tx")]
        public float TX;
        [XmlAttribute("ty")]
        public float TY;
    }
}