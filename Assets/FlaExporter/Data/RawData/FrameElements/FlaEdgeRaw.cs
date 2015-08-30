using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.FrameElements
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
    }
}