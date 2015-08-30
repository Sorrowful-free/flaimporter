using System.Collections.Generic;
using System.Xml.Serialization;
using Assets.FlaExporter.Data.RawData.FillStyles;
using Assets.FlaExporter.Data.RawData.StorkeStyle;

namespace Assets.FlaExporter.Data.RawData.FrameElements
{
    public class FlaShapeRaw : FlaFrameElementRaw
    {
        [XmlArray("fills")]
        [XmlArrayItem("FillStyle")]
        public List<FlaFillStyleRaw> FillStyles;

        [XmlArray("strokes")]
        [XmlArrayItem("StrokeStyle")]
        public List<FlaStorkeStyleRaw> StorkeStyles;

        [XmlArray("edges")] 
        [XmlArrayItem("Edge")] 
        public List<FlaEdgeRaw> Edges;
    }

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