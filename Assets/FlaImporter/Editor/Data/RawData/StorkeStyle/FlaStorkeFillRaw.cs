using System.Xml.Serialization;
using Assets.FlaImporter.Editor.Data.RawData.FillStyles.FillStyles;
using Assets.FlaImporter.Editor.Data.RawData.FillStyles.FillStyles.GradientFillStyles;

namespace Assets.FlaImporter.Editor.Data.RawData.StorkeStyle
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