using System.Xml.Serialization;

namespace Assets.FlaExporter.Editor.Data.RawData.Geom
{
    public class FlaEdgeRaw
    {
        [XmlAttribute("fillStyle0")] 
        public int FillStyle0;

        [XmlAttribute("fillStyle1")]
        public int FillStyle1;

        [XmlAttribute("strokeStyle")]
        public int StrokeStyle;

        [XmlAttribute("edges")]
        public string Edges;

        [XmlAttribute("cubics")]
        public string Cubics;
    }
}