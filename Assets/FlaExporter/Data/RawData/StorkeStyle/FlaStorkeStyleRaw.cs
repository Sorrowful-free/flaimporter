using System.Xml.Serialization;
using Assets.FlaExporter.Data.RawData.StorkeStyle.StorkeStyles;

namespace Assets.FlaExporter.Data.RawData.StorkeStyle
{
    public class FlaStorkeStyleRaw
    {
        [XmlAttribute("index")] 
        public int Index;

        [XmlElement("SolidStroke", typeof(FlaSolidStrokeStyleRaw))]
        [XmlElement("DashedStroke", typeof(FlaDashedStrokeStyleRaw))]
        [XmlElement("DottedStroke", typeof(FlaDottedStrokeStyleRaw))]
        [XmlElement("RaggedStroke", typeof(FlaRaggedStrokeStyleRaw))]
        [XmlElement("StippleStroke", typeof(FlaStippleStrokeStyleRaw))]
        public FlaBaseStorkyStyleRaw StorkyStyle;
    }
}