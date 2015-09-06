using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.Geom
{
    public class FlaMatrixRaw
    {
        [XmlAttribute("a")]
        public float A = 1;
        [XmlAttribute("b")]
        public float B = 0;
        [XmlAttribute("c")]
        public float C = 0;
        [XmlAttribute("d")]
        public float D = 1;
        [XmlAttribute("tx")]
        public float TX = 0;
        [XmlAttribute("ty")]
        public float TY = 0;
    }
}