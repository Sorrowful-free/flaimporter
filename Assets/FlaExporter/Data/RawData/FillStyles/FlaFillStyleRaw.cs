using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.FillStyles
{
    public class FlaFillStyleRaw
    {
        [XmlAttribute("index")] 
        public int Index;

        [XmlElement("SolidColor", typeof(FlaSolidColorFillStyleRaw))]
        [XmlElement("LinearGradient", typeof(FlaLinearGradientFillStyleRaw))]
        [XmlElement("RadialGradient", typeof(FlaRadialGradientFillStyleRaw))]
        public FlaBaseFillStyleRaw FillStyle;
    }
}