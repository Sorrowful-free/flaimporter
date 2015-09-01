using System.Xml.Serialization;
using Assets.FlaExporter.Data.RawData.FillStyles;
using Assets.FlaExporter.Data.RawData.FillStyles.FillStyles;
using Assets.FlaExporter.Data.RawData.FillStyles.FillStyles.GradientFillStyles;

namespace Assets.FlaExporter.Data.RawData.StorkeStyle
{
    public class FlaStorkeFillRaw
    {
        [XmlElement("SolidColor", Type = typeof(FlaSolidColorFillStyleRaw))]
        [XmlElement("LinearGradient", Type = typeof(FlaLinearGradientFillStyleRaw))]
        [XmlElement("RadialGradient", Type = typeof(FlaRadialGradientFillStyleRaw))]
        [XmlElement("BitmapFill", Type = typeof(FlaBitmapFillRaw))]
        public FlaBaseFillStyleRaw FillStyle;
    }
}