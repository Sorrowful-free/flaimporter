using System.Xml.Serialization;
using Assets.FlaExporter.Data.RawData.FillStyles.FillStyles;
using Assets.FlaExporter.Data.RawData.FillStyles.FillStyles.GradientFillStyles;

namespace Assets.FlaExporter.Data.RawData.FillStyles
{
    public class FlaFillStyleRaw
    {
        [XmlAttribute("index")] 
        public int Index;

        [XmlElement("SolidColor", typeof(FlaSolidColorFillStyleRaw))]
        [XmlElement("LinearGradient", typeof(FlaLinearGradientFillStyleRaw))]
        [XmlElement("RadialGradient", typeof(FlaRadialGradientFillStyleRaw))]
        [XmlElement("BitmapFill", typeof(FlaBitmapFillRaw))]
        public FlaBaseFillStyleRaw FillStyle;
    }
}