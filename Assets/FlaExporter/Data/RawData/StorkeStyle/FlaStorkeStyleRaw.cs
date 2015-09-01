using System.Xml.Serialization;
using Assets.FlaExporter.Data.RawData.StorkeStyle.StorkeStyles;

namespace Assets.FlaExporter.Data.RawData.StorkeStyle
{
    public class FlaStorkeStyleRaw
    {
        [XmlAttribute("index")] 
        public int Index;

        [XmlElement("SolidStroke", Type = typeof(FlaSolidStrokeStyleRaw))]
        [XmlElement("DashedStroke", Type = typeof(FlaDashedStrokeStyleRaw))]
        [XmlElement("DottedStroke", Type = typeof(FlaDottedStrokeStyleRaw))]
        [XmlElement("RaggedStroke", Type = typeof(FlaRaggedStrokeStyleRaw))]
        [XmlElement("StippleStroke", Type = typeof(FlaStippleStrokeStyleRaw))]
        public FlaBaseStorkyStyleRaw StorkyStyle;
    }
}