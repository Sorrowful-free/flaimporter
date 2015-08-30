using System.Xml.Serialization;
using Assets.FlaExporter.Data.RawData.FillStyles;

namespace Assets.FlaExporter.Data.RawData.StorkeStyle
{
    public class FlaStorkeFillRaw
    {
        [XmlElement("SolidColor", typeof(FlaSolidColorFillStyleRaw))]
        [XmlElement("LinearGradient", typeof(FlaLinearGradientFillStyleRaw))]
        [XmlElement("RadialGradient", typeof(FlaRadialGradientFillStyleRaw))]
        public FlaBaseFillStyleRaw FillStyle;
    }
}